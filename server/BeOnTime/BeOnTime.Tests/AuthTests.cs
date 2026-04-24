using Moq;
using Xunit;
using BeOnTime.Application.Services;
using BeOnTime.Application.Interfaces;
using BeOnTime.Application.DTOs;
using BeOnTime.Application.Options;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace BeOnTime.Tests
{
    public class AuthTests
    {
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
    }
}