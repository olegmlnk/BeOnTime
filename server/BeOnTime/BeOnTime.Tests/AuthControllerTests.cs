using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using BeOnTime.API.Controllers;
using BeOnTime.Application.Interfaces;
using BeOnTime.Application.DTOs;

namespace BeOnTime.Tests
{
    public class AuthControllerTests
    {
        private AuthController CreateController(Mock<IAuthService> mockService, string? userAgent = "TestAgent")
        {
            var controller = new AuthController(mockService.Object);
            var httpContext = new DefaultHttpContext();
            if (userAgent != null)
                httpContext.Request.Headers["User-Agent"] = userAgent;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            return controller;
        }

        [Fact]
        public async Task Register_ShouldReturnOk_WhenRegistrationSucceeds()
        {
            var mockService = new Mock<IAuthService>();
            var token = new TokenResponseDto
            {
                AccessToken = "access-token",
                RefreshToken = "refresh-token",
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15)
            };

            mockService
                .Setup(s => s.RegisterAsync(It.IsAny<RegisterRequestDto>(), It.IsAny<string?>()))
                .ReturnsAsync(AuthResult.Ok(token));

            var controller = CreateController(mockService);
            var request = new RegisterRequestDto
            {
                UserName = "testuser",
                Email = "test@test.com",
                Password = "Password123!"
            };

            var result = await controller.Register(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedToken = Assert.IsType<TokenResponseDto>(okResult.Value);
            Assert.Equal("access-token", returnedToken.AccessToken);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenRegistrationFails()
        {
            var mockService = new Mock<IAuthService>();

            mockService
                .Setup(s => s.RegisterAsync(It.IsAny<RegisterRequestDto>(), It.IsAny<string?>()))
                .ReturnsAsync(AuthResult.Fail("User already exists"));

            var controller = CreateController(mockService);
            var request = new RegisterRequestDto
            {
                UserName = "testuser",
                Email = "test@test.com",
                Password = "Password123!"
            };

            var result = await controller.Register(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            var mockService = new Mock<IAuthService>();
            var token = new TokenResponseDto
            {
                AccessToken = "access-token",
                RefreshToken = "refresh-token",
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15)
            };

            mockService
                .Setup(s => s.LoginASync(It.IsAny<LoginRequestDto>(), It.IsAny<string?>()))
                .ReturnsAsync(AuthResult.Ok(token));

            var controller = CreateController(mockService);
            var request = new LoginRequestDto
            {
                Email = "test@test.com",
                Password = "Password123!"
            };

            var result = await controller.Login(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedToken = Assert.IsType<TokenResponseDto>(okResult.Value);
            Assert.Equal("access-token", returnedToken.AccessToken);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            var mockService = new Mock<IAuthService>();

            mockService
                .Setup(s => s.LoginASync(It.IsAny<LoginRequestDto>(), It.IsAny<string?>()))
                .ReturnsAsync(AuthResult.Fail("Invalid credentials"));

            var controller = CreateController(mockService);
            var request = new LoginRequestDto
            {
                Email = "test@test.com",
                Password = "WrongPassword"
            };

            var result = await controller.Login(request);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Refresh_ShouldReturnOk_WhenTokenIsValid()
        {
            var mockService = new Mock<IAuthService>();
            var token = new TokenResponseDto
            {
                AccessToken = "new-access-token",
                RefreshToken = "new-refresh-token",
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15)
            };

            mockService
                .Setup(s => s.RefreshTokenAsync(It.IsAny<RefreshTokenRequestDto>(), It.IsAny<string?>()))
                .ReturnsAsync(AuthResult.Ok(token));

            var controller = CreateController(mockService);
            var request = new RefreshTokenRequestDto { RefreshToken = "valid-token" };

            var result = await controller.Refresh(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedToken = Assert.IsType<TokenResponseDto>(okResult.Value);
            Assert.Equal("new-access-token", returnedToken.AccessToken);
        }

        [Fact]
        public async Task Refresh_ShouldReturnUnauthorized_WhenTokenIsInvalid()
        {
            var mockService = new Mock<IAuthService>();

            mockService
                .Setup(s => s.RefreshTokenAsync(It.IsAny<RefreshTokenRequestDto>(), It.IsAny<string?>()))
                .ReturnsAsync(AuthResult.Fail("Invalid or expired refresh token"));

            var controller = CreateController(mockService);
            var request = new RefreshTokenRequestDto { RefreshToken = "expired-token" };

            var result = await controller.Refresh(request);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}
