using Moq;
using Xunit;
using BeOnTime.Application.Services;
using BeOnTime.Application.Interfaces;
using BeOnTime.Core.Entities;

namespace BeOnTime.Tests
{
    public class SessionServiceTests
    {
        [Fact]
        public async Task GetSessionsAsync_ShouldReturnMappedSessions()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IRefreshTokenRepository>();
            var service = new SessionService(mockRepo.Object);

            var userId = Guid.NewGuid();
            var currentToken = "current-refresh-token";

            var tokens = new List<RefreshToken>
            {
                new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    Token = currentToken,
                    UserAgent = "Chrome/120",
                    CreatedAt = DateTime.UtcNow.AddHours(-1),
                    Expires = DateTime.UtcNow.AddDays(7),
                    IsRevoked = false,
                    UserId = userId
                },
                new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    Token = "other-token",
                    UserAgent = "Firefox/119",
                    CreatedAt = DateTime.UtcNow.AddHours(-2),
                    Expires = DateTime.UtcNow.AddDays(7),
                    IsRevoked = false,
                    UserId = userId
                }
            };

            mockRepo
                .Setup(r => r.GetActiveByUserIdAsync(userId))
                .ReturnsAsync(tokens);

            // 2. ACT
            var result = await service.GetSessionsAsync(userId, currentToken);

            // 3. ASSERT
            Assert.Equal(2, result.Count);

            // Перша сесія — поточна
            Assert.True(result[0].IsCurrent);
            Assert.Equal("Chrome/120", result[0].UserAgent);

            // Друга сесія — не поточна
            Assert.False(result[1].IsCurrent);
            Assert.Equal("Firefox/119", result[1].UserAgent);
        }

        [Fact]
        public async Task GetSessionsAsync_ShouldSetIsCurrentFalse_WhenCurrentTokenIsNull()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IRefreshTokenRepository>();
            var service = new SessionService(mockRepo.Object);

            var userId = Guid.NewGuid();

            var tokens = new List<RefreshToken>
            {
                new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    Token = "some-token",
                    UserId = userId,
                    Expires = DateTime.UtcNow.AddDays(7)
                }
            };

            mockRepo
                .Setup(r => r.GetActiveByUserIdAsync(userId))
                .ReturnsAsync(tokens);

            // 2. ACT — передаємо null як поточний токен
            var result = await service.GetSessionsAsync(userId, null);

            // 3. ASSERT — жодна сесія не позначена як поточна
            Assert.Single(result);
            Assert.False(result[0].IsCurrent);
        }

        [Fact]
        public async Task RevokeAsync_ShouldReturnFalse_WhenSessionDoesNotExist()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IRefreshTokenRepository>();
            var service = new SessionService(mockRepo.Object);

            var userId = Guid.NewGuid();

            mockRepo
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<Guid>()))
                .ReturnsAsync((RefreshToken?)null);

            // 2. ACT
            var result = await service.RevokeAsync(userId, Guid.NewGuid());

            // 3. ASSERT
            Assert.False(result);
        }

        [Fact]
        public async Task RevokeAsync_ShouldRevokeAndReturnTrue_WhenSessionExistsAndNotRevoked()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IRefreshTokenRepository>();
            var service = new SessionService(mockRepo.Object);

            var userId = Guid.NewGuid();
            var sessionId = Guid.NewGuid();

            var token = new RefreshToken
            {
                Id = sessionId,
                UserId = userId,
                Token = "token-to-revoke",
                IsRevoked = false
            };

            mockRepo
                .Setup(r => r.GetByIdAsync(userId, sessionId))
                .ReturnsAsync(token);

            // 2. ACT
            var result = await service.RevokeAsync(userId, sessionId);

            // 3. ASSERT
            Assert.True(result);
            Assert.True(token.IsRevoked);
            mockRepo.Verify(r => r.UpdateAsync(token), Times.Once);
        }

        [Fact]
        public async Task RevokeAsync_ShouldReturnTrue_WhenAlreadyRevoked_WithoutCallingUpdate()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IRefreshTokenRepository>();
            var service = new SessionService(mockRepo.Object);

            var userId = Guid.NewGuid();
            var sessionId = Guid.NewGuid();

            var token = new RefreshToken
            {
                Id = sessionId,
                UserId = userId,
                Token = "already-revoked-token",
                IsRevoked = true // Вже відкликаний
            };

            mockRepo
                .Setup(r => r.GetByIdAsync(userId, sessionId))
                .ReturnsAsync(token);

            // 2. ACT
            var result = await service.RevokeAsync(userId, sessionId);

            // 3. ASSERT
            Assert.True(result);
            // UpdateAsync НЕ повинен викликатися, бо токен вже revoked
            mockRepo.Verify(r => r.UpdateAsync(It.IsAny<RefreshToken>()), Times.Never);
        }

        [Fact]
        public async Task RevokeAllAsync_ShouldDelegateToRepository()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IRefreshTokenRepository>();
            var service = new SessionService(mockRepo.Object);

            var userId = Guid.NewGuid();
            var exceptToken = "keep-this-token";

            mockRepo
                .Setup(r => r.RevokeAllForUserAsync(userId, exceptToken))
                .ReturnsAsync(3); // 3 сесії відкликано

            // 2. ACT
            var result = await service.RevokeAllAsync(userId, exceptToken);

            // 3. ASSERT
            Assert.Equal(3, result);
            mockRepo.Verify(r => r.RevokeAllForUserAsync(userId, exceptToken), Times.Once);
        }
    }
}
