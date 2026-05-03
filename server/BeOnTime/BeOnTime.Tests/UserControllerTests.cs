using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;
using BeOnTime.API.Controllers;
using BeOnTime.Application.Interfaces;
using BeOnTime.Core.Entities;
using BeOnTime.Core.Enums;

namespace BeOnTime.Tests
{
    public class UserControllerTests
    {
        private UserController CreateControllerWithUser(Mock<IUserRepository> mockRepo, Guid userId)
        {
            var controller = new UserController(mockRepo.Object);
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
            return controller;
        }

        [Fact]
        public async Task GetCurrentUser_ShouldReturnOk_WhenUserExists()
        {
            var mockRepo = new Mock<IUserRepository>();
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                UserName = "testuser",
                Email = "test@test.com",
                Role = Role.User,
                CreatedAt = DateTime.UtcNow
            };
            mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            var controller = CreateControllerWithUser(mockRepo, userId);

            var result = await controller.GetCurrentUser();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetCurrentUser_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            var mockRepo = new Mock<IUserRepository>();
            var userId = Guid.NewGuid();
            mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);
            var controller = CreateControllerWithUser(mockRepo, userId);

            var result = await controller.GetCurrentUser();

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetCurrentUser_ShouldReturnUnauthorized_WhenNoValidClaim()
        {
            var mockRepo = new Mock<IUserRepository>();
            var controller = new UserController(mockRepo.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await controller.GetCurrentUser();

            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
