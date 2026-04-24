using Moq;
using Xunit;
using BeOnTime.Application.Services;
using BeOnTime.Application.Interfaces;
using BeOnTime.Application.DTOs;
using BeOnTime.Application.Options;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using BeOnTime.Core.Entities;

namespace BeOnTime.Tests
{
    public class AuthTests
    {
        [Fact]
        public async Task RegisterAsync_ShouldReturnSuccess_WhenDataIsValid()
        {
            // 1. ARRANGE
            var mockUserRepository = new Mock<IUserRepository>();
            var mockJwtTokenService = new Mock<IJwtTokenService>();
            
            // Налаштування AuthService в конструкторі читає _jwtOptions = jwtOptions.Value
            // Тому треба створити фейкові налаштування, щоб код не впав з помилкою
            var jwtOptions = new JwtOptions { ExpirationTimeInMinutes = 15 };
            var mockJwtOptions = new Mock<IOptions<JwtOptions>>();
            mockJwtOptions.Setup(opt => opt.Value).Returns(jwtOptions);

            // Налаштування БД: кажемо, що такого юзера ще НЕМАЄ (повертаємо false)
            mockUserRepository
                .Setup(repo => repo.ExistsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            // Налаштування генератора токенів: повертаємо зрозумілі рядки
            mockJwtTokenService
                .Setup(jwt => jwt.GenerateAccessToken(It.IsAny<User>()))
                .Returns("fake-access-token");
            mockJwtTokenService
                .Setup(jwt => jwt.GenerateRefreshToken())
                .Returns("fake-refresh-token");

            // Створення сервісу з фейковими залежностями
            var authService = new AuthService(
                mockUserRepository.Object,
                mockJwtTokenService.Object,
                mockJwtOptions.Object
            );

            var request = new RegisterRequestDto
            {
                Email = "newuser@test.com",
                UserName = "newuser",
                Password = "ValidPassword123!"
            };

            // 2. ACT
            var result = await authService.RegisterAsync(request, "Test-Agent");

            // 3. ASSERT
            // Перевірка чи успішна відповідь і чи згенерувався токен
            Assert.True(result.Success);
            Assert.NotNull(result.Token);
            Assert.Equal("fake-access-token", result.Token.AccessToken);

            // Перевірка чи викликався метод CreateAsync для БД рівно 1 раз!
            mockUserRepository.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Once);
        }
        [Fact]
        public async Task RegisterAsync_ShouldReturnFail_WhenUserAlreadyExists()
        {
            // 1. ARRANGE
            var mockUserRepository = new Mock<IUserRepository>();
            var mockJwtTokenService = new Mock<IJwtTokenService>();
            var mockJwtOptions = new Mock<IOptions<JwtOptions>>();

            // Налаштування фейку: імітація юзера в БД
            mockUserRepository
                .Setup(repo => repo.ExistsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Створення сервісу з фейковими залежностями
            var authService = new AuthService(
                mockUserRepository.Object,
                mockJwtTokenService.Object,
                mockJwtOptions.Object
            );

            // Формування валідного запиту (пароль > 8 символів)
            var request = new RegisterRequestDto
            {
                Email = "[EMAIL_ADDRESS]",
                UserName = "ivan_r",
                Password = "StrongPassword123!"
            };

            // 2. ACT
            var result = await authService.RegisterAsync(request, "Test-Agent");

            // 3. ASSERT
            // Перевірка точних властивостей з AuthResult.cs
            Assert.False(result.Success);
            Assert.Equal("User with this email or username already exists", result.Error);
        }
        [Fact]
        public async Task LoginASync_ShouldReturnFail_WhenUserDoesNotExist()
        {
            // 1. ARRANGE
            var mockUserRepository = new Mock<IUserRepository>();
            var mockJwtTokenService = new Mock<IJwtTokenService>();
            var mockJwtOptions = new Mock<IOptions<JwtOptions>>();

            // Налаштовуємо БД: коли сервіс шукатиме імейл, повертаємо null (нікого не знайдено)
            mockUserRepository
                .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            var authService = new AuthService(
                mockUserRepository.Object,
                mockJwtTokenService.Object,
                mockJwtOptions.Object
            );

            var request = new LoginRequestDto 
            { 
                Email = "ghost@test.com", 
                Password = "SomePassword123!" 
            };

            // 2. ACT
            var result = await authService.LoginASync(request, "Test-Agent");

            // 3. ASSERT
            Assert.False(result.Success);
            Assert.Equal("Invalid credentials", result.Error);
        }

        [Fact]
        public async Task LoginASync_ShouldReturnFail_WhenPasswordIsIncorrect()
        {
            // 1. ARRANGE
            var mockUserRepository = new Mock<IUserRepository>();
            var mockJwtTokenService = new Mock<IJwtTokenService>();
            var mockJwtOptions = new Mock<IOptions<JwtOptions>>();

            // Створення фейкового юзера
            var fakeUserInDb = new User 
            { 
                Email = "realuser@test.com", 
                PasswordHash = "$2a$11$SomeFakeHashedPasswordString..." 
            };

            // Налаштування БД: тепер вона знаходить юзера
            mockUserRepository
                .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(fakeUserInDb);

            var authService = new AuthService(
                mockUserRepository.Object,
                mockJwtTokenService.Object,
                mockJwtOptions.Object
            );

            // Клієнт вводить ПРАВИЛЬНИЙ імейл, але НЕПРАВИЛЬНИЙ пароль
            var request = new LoginRequestDto 
            { 
                Email = "realuser@test.com", 
                Password = "WrongPassword123!" 
            };

            // 2. ACT
            var result = await authService.LoginASync(request, "Test-Agent");

            // 3. ASSERT
            Assert.False(result.Success);
            Assert.Equal("Invalid credentials", result.Error);
        }
    }
}