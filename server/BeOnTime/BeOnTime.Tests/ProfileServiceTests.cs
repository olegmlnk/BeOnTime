using Moq;
using Xunit;
using BeOnTime.Application.Services;
using BeOnTime.Application.Interfaces;
using BeOnTime.Application.DTOs.Profile;
using BeOnTime.Core.Entities;
using BeOnTime.Core.Enums;

namespace BeOnTime.Tests
{
    public class ProfileServiceTests
    {
        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // 1. ARRANGE
            var mockUsers = new Mock<IUserRepository>();
            var mockTokens = new Mock<IRefreshTokenRepository>();
            var service = new ProfileService(mockUsers.Object, mockTokens.Object);

            mockUsers
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User?)null);

            // 2. ACT
            var result = await service.GetAsync(Guid.NewGuid());

            // 3. ASSERT
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnMappedProfile_WhenUserExists()
        {
            // 1. ARRANGE
            var mockUsers = new Mock<IUserRepository>();
            var mockTokens = new Mock<IRefreshTokenRepository>();
            var service = new ProfileService(mockUsers.Object, mockTokens.Object);

            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                UserName = "testuser",
                Email = "test@example.com",
                Role = Role.User,
                CreatedAt = DateTime.UtcNow
            };

            mockUsers
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // 2. ACT
            var result = await service.GetAsync(userId);

            // 3. ASSERT
            Assert.NotNull(result);
            Assert.Equal("testuser", result.UserName);
            Assert.Equal("test@example.com", result.Email);
            Assert.Equal(Role.User, result.Role);
        }

        [Fact]
        public async Task UpdateUsernameAsync_ShouldReturnFail_WhenUsernameTooShort()
        {
            // 1. ARRANGE
            var mockUsers = new Mock<IUserRepository>();
            var mockTokens = new Mock<IRefreshTokenRepository>();
            var service = new ProfileService(mockUsers.Object, mockTokens.Object);

            var dto = new UpdateUsernameDto { UserName = "ab" }; // Менше 3 символів

            // 2. ACT
            var result = await service.UpdateUsernameAsync(Guid.NewGuid(), dto);

            // 3. ASSERT
            Assert.False(result.Success);
            Assert.Equal("Username must be at least 3 characters", result.Error);
        }

        [Fact]
        public async Task UpdateUsernameAsync_ShouldReturnFail_WhenUserNotFound()
        {
            // 1. ARRANGE
            var mockUsers = new Mock<IUserRepository>();
            var mockTokens = new Mock<IRefreshTokenRepository>();
            var service = new ProfileService(mockUsers.Object, mockTokens.Object);

            mockUsers
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User?)null);

            var dto = new UpdateUsernameDto { UserName = "validname" };

            // 2. ACT
            var result = await service.UpdateUsernameAsync(Guid.NewGuid(), dto);

            // 3. ASSERT
            Assert.False(result.Success);
            Assert.Equal("User not found", result.Error);
        }

        [Fact]
        public async Task UpdateUsernameAsync_ShouldReturnFail_WhenUsernameAlreadyTaken()
        {
            // 1. ARRANGE
            var mockUsers = new Mock<IUserRepository>();
            var mockTokens = new Mock<IRefreshTokenRepository>();
            var service = new ProfileService(mockUsers.Object, mockTokens.Object);

            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                UserName = "oldname",
                Email = "user@example.com"
            };

            mockUsers
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);
            mockUsers
                .Setup(r => r.ExistsExcludingAsync(user.Email, "takenname", userId))
                .ReturnsAsync(true); // Зайнято!

            var dto = new UpdateUsernameDto { UserName = "takenname" };

            // 2. ACT
            var result = await service.UpdateUsernameAsync(userId, dto);

            // 3. ASSERT
            Assert.False(result.Success);
            Assert.Equal("Username already taken", result.Error);
        }

        [Fact]
        public async Task UpdateUsernameAsync_ShouldReturnSuccess_WhenDataIsValid()
        {
            // 1. ARRANGE
            var mockUsers = new Mock<IUserRepository>();
            var mockTokens = new Mock<IRefreshTokenRepository>();
            var service = new ProfileService(mockUsers.Object, mockTokens.Object);

            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                UserName = "oldname",
                Email = "user@example.com"
            };

            mockUsers
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);
            mockUsers
                .Setup(r => r.ExistsExcludingAsync(user.Email, "newname", userId))
                .ReturnsAsync(false); // Вільно!

            var dto = new UpdateUsernameDto { UserName = "newname" };

            // 2. ACT
            var result = await service.UpdateUsernameAsync(userId, dto);

            // 3. ASSERT
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.Equal("newname", user.UserName);
            mockUsers.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateEmailAsync_ShouldReturnFail_WhenEmailIsEmpty()
        {
            // 1. ARRANGE
            var mockUsers = new Mock<IUserRepository>();
            var mockTokens = new Mock<IRefreshTokenRepository>();
            var service = new ProfileService(mockUsers.Object, mockTokens.Object);

            var dto = new UpdateEmailDto { Email = "", CurrentPassword = "password" };

            // 2. ACT
            var result = await service.UpdateEmailAsync(Guid.NewGuid(), dto);

            // 3. ASSERT
            Assert.False(result.Success);
            Assert.Equal("Email is required", result.Error);
        }

        [Fact]
        public async Task UpdateEmailAsync_ShouldReturnFail_WhenPasswordIsIncorrect()
        {
            // 1. ARRANGE
            var mockUsers = new Mock<IUserRepository>();
            var mockTokens = new Mock<IRefreshTokenRepository>();
            var service = new ProfileService(mockUsers.Object, mockTokens.Object);

            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Email = "old@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword")
            };

            mockUsers
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            var dto = new UpdateEmailDto
            {
                Email = "new@example.com",
                CurrentPassword = "wrongpassword" // Невірний пароль!
            };

            // 2. ACT
            var result = await service.UpdateEmailAsync(userId, dto);

            // 3. ASSERT
            Assert.False(result.Success);
            Assert.Equal("Invalid current password", result.Error);
        }

        [Fact]
        public async Task UpdateEmailAsync_ShouldReturnSuccess_WhenDataIsValid()
        {
            // 1. ARRANGE
            var mockUsers = new Mock<IUserRepository>();
            var mockTokens = new Mock<IRefreshTokenRepository>();
            var service = new ProfileService(mockUsers.Object, mockTokens.Object);

            var userId = Guid.NewGuid();
            var correctPassword = "correctpassword";
            var user = new User
            {
                Id = userId,
                UserName = "testuser",
                Email = "old@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(correctPassword)
            };

            mockUsers
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);
            mockUsers
                .Setup(r => r.ExistsExcludingAsync("new@example.com", user.UserName, userId))
                .ReturnsAsync(false);

            var dto = new UpdateEmailDto
            {
                Email = "new@example.com",
                CurrentPassword = correctPassword
            };

            // 2. ACT
            var result = await service.UpdateEmailAsync(userId, dto);

            // 3. ASSERT
            Assert.True(result.Success);
            Assert.Equal("new@example.com", user.Email);
            mockUsers.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldReturnFail_WhenPasswordsDoNotMatch()
        {
            // 1. ARRANGE
            var mockUsers = new Mock<IUserRepository>();
            var mockTokens = new Mock<IRefreshTokenRepository>();
            var service = new ProfileService(mockUsers.Object, mockTokens.Object);

            var dto = new ChangePasswordDto
            {
                CurrentPassword = "current",
                NewPassword = "newpassword1",
                ConfirmNewPassword = "newpassword2" // Не збігається!
            };

            // 2. ACT
            var result = await service.ChangePasswordAsync(Guid.NewGuid(), dto);

            // 3. ASSERT
            Assert.False(result.Success);
            Assert.Equal("New password and confirmation do not match", result.Error);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldReturnFail_WhenPasswordTooShort()
        {
            // 1. ARRANGE
            var mockUsers = new Mock<IUserRepository>();
            var mockTokens = new Mock<IRefreshTokenRepository>();
            var service = new ProfileService(mockUsers.Object, mockTokens.Object);

            var dto = new ChangePasswordDto
            {
                CurrentPassword = "current",
                NewPassword = "short",         // Менше 8 символів
                ConfirmNewPassword = "short"
            };

            // 2. ACT
            var result = await service.ChangePasswordAsync(Guid.NewGuid(), dto);

            // 3. ASSERT
            Assert.False(result.Success);
            Assert.Equal("New password must be at least 8 characters", result.Error);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldReturnFail_WhenCurrentPasswordIsIncorrect()
        {
            // 1. ARRANGE
            var mockUsers = new Mock<IUserRepository>();
            var mockTokens = new Mock<IRefreshTokenRepository>();
            var service = new ProfileService(mockUsers.Object, mockTokens.Object);

            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("realpassword")
            };

            mockUsers
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            var dto = new ChangePasswordDto
            {
                CurrentPassword = "wrongpassword", // Невірний поточний пароль!
                NewPassword = "newstrongpassword",
                ConfirmNewPassword = "newstrongpassword"
            };

            // 2. ACT
            var result = await service.ChangePasswordAsync(userId, dto);

            // 3. ASSERT
            Assert.False(result.Success);
            Assert.Equal("Invalid current password", result.Error);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldHashNewPasswordAndRevokeTokens_WhenDataIsValid()
        {
            // 1. ARRANGE
            var mockUsers = new Mock<IUserRepository>();
            var mockTokens = new Mock<IRefreshTokenRepository>();
            var service = new ProfileService(mockUsers.Object, mockTokens.Object);

            var userId = Guid.NewGuid();
            var currentPassword = "currentpassword";
            var user = new User
            {
                Id = userId,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(currentPassword)
            };

            mockUsers
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            var dto = new ChangePasswordDto
            {
                CurrentPassword = currentPassword,
                NewPassword = "brandnewpassword",
                ConfirmNewPassword = "brandnewpassword"
            };

            // 2. ACT
            var result = await service.ChangePasswordAsync(userId, dto);

            // 3. ASSERT
            Assert.True(result.Success);

            // Перевірка, що пароль змінився (новий хеш — валідний)
            Assert.True(BCrypt.Net.BCrypt.Verify("brandnewpassword", user.PasswordHash));

            // Перевірка, що всі refresh-токени відкликано (безпека)
            mockTokens.Verify(r => r.RevokeAllForUserAsync(userId, null), Times.Once);

            mockUsers.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task DeleteAccountAsync_ShouldReturnFail_WhenUserNotFound()
        {
            // 1. ARRANGE
            var mockUsers = new Mock<IUserRepository>();
            var mockTokens = new Mock<IRefreshTokenRepository>();
            var service = new ProfileService(mockUsers.Object, mockTokens.Object);

            mockUsers
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User?)null);

            var dto = new DeleteAccountDto { CurrentPassword = "password" };

            // 2. ACT
            var result = await service.DeleteAccountAsync(Guid.NewGuid(), dto);

            // 3. ASSERT
            Assert.False(result.Success);
            Assert.Equal("User not found", result.Error);
        }

        [Fact]
        public async Task DeleteAccountAsync_ShouldReturnFail_WhenPasswordIsIncorrect()
        {
            // 1. ARRANGE
            var mockUsers = new Mock<IUserRepository>();
            var mockTokens = new Mock<IRefreshTokenRepository>();
            var service = new ProfileService(mockUsers.Object, mockTokens.Object);

            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword")
            };

            mockUsers
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            var dto = new DeleteAccountDto { CurrentPassword = "wrongpassword" };

            // 2. ACT
            var result = await service.DeleteAccountAsync(userId, dto);

            // 3. ASSERT
            Assert.False(result.Success);
            Assert.Equal("Invalid current password", result.Error);
        }

        [Fact]
        public async Task DeleteAccountAsync_ShouldSoftDeleteAndRevokeTokens_WhenDataIsValid()
        {
            // 1. ARRANGE
            var mockUsers = new Mock<IUserRepository>();
            var mockTokens = new Mock<IRefreshTokenRepository>();
            var service = new ProfileService(mockUsers.Object, mockTokens.Object);

            var userId = Guid.NewGuid();
            var password = "validpassword";
            var user = new User
            {
                Id = userId,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            mockUsers
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            var dto = new DeleteAccountDto { CurrentPassword = password };

            // 2. ACT
            var result = await service.DeleteAccountAsync(userId, dto);

            // 3. ASSERT
            Assert.True(result.Success);

            // Soft-delete: DeletedAt проставлений
            Assert.NotNull(user.DeletedAt);

            // Всі сесії відкликані
            mockTokens.Verify(r => r.RevokeAllForUserAsync(userId, null), Times.Once);
            mockUsers.Verify(r => r.UpdateAsync(user), Times.Once);
        }
    }
}
