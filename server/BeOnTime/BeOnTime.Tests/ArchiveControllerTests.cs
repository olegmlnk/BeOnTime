using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;
using BeOnTime.API.Controllers;
using BeOnTime.Application.Interfaces;
using BeOnTime.Application.DTOs.Archive;

namespace BeOnTime.Tests
{
    public class ArchiveControllerTests
    {
        private ArchiveController CreateControllerWithUser(Mock<IArchiveService> mockService, Guid userId)
        {
            var controller = new ArchiveController(mockService.Object);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            return controller;
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithItems()
        {
            var mockService = new Mock<IArchiveService>();
            var userId = Guid.NewGuid();

            var items = new List<ArchiveItemDto>
            {
                new ArchiveItemDto { Id = Guid.NewGuid(), Type = "task", Title = "Archived task" },
                new ArchiveItemDto { Id = Guid.NewGuid(), Type = "idea", Title = "Archived idea" }
            };

            mockService
                .Setup(s => s.GetAllAsync(userId))
                .ReturnsAsync(items);

            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedItems = Assert.IsAssignableFrom<IReadOnlyList<ArchiveItemDto>>(okResult.Value);
            Assert.Equal(2, returnedItems.Count);
        }

        [Fact]
        public async Task Restore_ShouldReturnNoContent_WhenRestoreSucceeds()
        {
            var mockService = new Mock<IArchiveService>();
            var userId = Guid.NewGuid();
            var itemId = Guid.NewGuid();

            mockService
                .Setup(s => s.RestoreAsync(userId, "task", itemId))
                .ReturnsAsync(true);

            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Restore("task", itemId);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Restore_ShouldReturnNotFound_WhenRestoreFails()
        {
            var mockService = new Mock<IArchiveService>();
            var userId = Guid.NewGuid();
            var itemId = Guid.NewGuid();

            mockService
                .Setup(s => s.RestoreAsync(userId, "task", itemId))
                .ReturnsAsync(false);

            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Restore("task", itemId);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
