using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;
using BeOnTime.API.Controllers;
using BeOnTime.Application.Interfaces;
using BeOnTime.Application.DTOs.Sessions;

namespace BeOnTime.Tests
{
    public class SessionsControllerTests
    {
        private SessionsController CreateControllerWithUser(Mock<ISessionService> mockService, Guid userId)
        {
            var controller = new SessionsController(mockService.Object);
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
            return controller;
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithSessions()
        {
            var mockService = new Mock<ISessionService>();
            var userId = Guid.NewGuid();
            var sessions = new List<SessionDto>
            {
                new SessionDto { Id = Guid.NewGuid(), IsCurrent = true },
                new SessionDto { Id = Guid.NewGuid(), IsCurrent = false }
            };
            mockService.Setup(s => s.GetSessionsAsync(userId, "current-token")).ReturnsAsync(sessions);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.GetAll("current-token");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<IReadOnlyList<SessionDto>>(okResult.Value);
            Assert.Equal(2, returned.Count);
        }

        [Fact]
        public async Task Revoke_ShouldReturnNoContent_WhenSucceeds()
        {
            var mockService = new Mock<ISessionService>();
            var userId = Guid.NewGuid();
            var sessionId = Guid.NewGuid();
            mockService.Setup(s => s.RevokeAsync(userId, sessionId)).ReturnsAsync(true);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Revoke(sessionId);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Revoke_ShouldReturnNotFound_WhenFails()
        {
            var mockService = new Mock<ISessionService>();
            var userId = Guid.NewGuid();
            var sessionId = Guid.NewGuid();
            mockService.Setup(s => s.RevokeAsync(userId, sessionId)).ReturnsAsync(false);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Revoke(sessionId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task RevokeAll_ShouldReturnOk_WithRevokedCount()
        {
            var mockService = new Mock<ISessionService>();
            var userId = Guid.NewGuid();
            mockService.Setup(s => s.RevokeAllAsync(userId, "keep-this-token")).ReturnsAsync(3);
            var controller = CreateControllerWithUser(mockService, userId);

            var request = new SessionsController.RevokeAllRequest { ExceptRefreshToken = "keep-this-token" };

            var result = await controller.RevokeAll(request);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task RevokeAll_ShouldReturnOk_WithNullRequest()
        {
            var mockService = new Mock<ISessionService>();
            var userId = Guid.NewGuid();
            mockService.Setup(s => s.RevokeAllAsync(userId, null)).ReturnsAsync(5);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.RevokeAll(null);

            Assert.IsType<OkObjectResult>(result);
        }
    }
}
