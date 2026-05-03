using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;
using BeOnTime.API.Controllers;
using BeOnTime.Application.Interfaces;
using BeOnTime.Application.DTOs.Settings;

namespace BeOnTime.Tests
{
    public class SettingsControllerTests
    {
        private SettingsController CreateControllerWithUser(Mock<IUserSettingsService> mockService, Guid userId)
        {
            var controller = new SettingsController(mockService.Object);
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
            return controller;
        }

        [Fact]
        public async Task Get_ShouldReturnOk_WithSettings()
        {
            var mockService = new Mock<IUserSettingsService>();
            var userId = Guid.NewGuid();
            var settings = new UserSettingsDto { Id = userId };
            mockService.Setup(s => s.GetAsync(userId)).ReturnsAsync(settings);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<UserSettingsDto>(okResult.Value);
            Assert.Equal(userId, returned.Id);
        }

        [Fact]
        public async Task Update_ShouldReturnOk_WithUpdatedSettings()
        {
            var mockService = new Mock<IUserSettingsService>();
            var userId = Guid.NewGuid();
            var dto = new UpdateUserSettingsDto { TimeZone = "UTC" };
            var updated = new UserSettingsDto { Id = userId, TimeZone = "UTC" };
            mockService.Setup(s => s.UpdateAsync(userId, dto)).ReturnsAsync(updated);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Update(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<UserSettingsDto>(okResult.Value);
            Assert.Equal("UTC", returned.TimeZone);
        }

        [Fact]
        public async Task GetHistory_ShouldReturnOk_WithHistoryList()
        {
            var mockService = new Mock<IUserSettingsService>();
            var userId = Guid.NewGuid();
            var history = new List<UserSettingsHistoryDto>
            {
                new UserSettingsHistoryDto { Id = Guid.NewGuid() },
                new UserSettingsHistoryDto { Id = Guid.NewGuid() }
            };
            mockService.Setup(s => s.GetHistoryAsync(userId)).ReturnsAsync(history);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.GetHistory();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<IReadOnlyList<UserSettingsHistoryDto>>(okResult.Value);
            Assert.Equal(2, returned.Count);
        }

        [Fact]
        public async Task Restore_ShouldReturnOk_WhenSucceeds()
        {
            var mockService = new Mock<IUserSettingsService>();
            var userId = Guid.NewGuid();
            var historyId = Guid.NewGuid();
            var settings = new UserSettingsDto { Id = userId };
            mockService.Setup(s => s.RestoreFromHistoryAsync(userId, historyId)).ReturnsAsync(SettingsResult<UserSettingsDto>.Ok(settings));
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Restore(historyId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<UserSettingsDto>(okResult.Value);
        }

        [Fact]
        public async Task Restore_ShouldReturnNotFound_WhenHistoryNotFound()
        {
            var mockService = new Mock<IUserSettingsService>();
            var userId = Guid.NewGuid();
            var historyId = Guid.NewGuid();
            mockService.Setup(s => s.RestoreFromHistoryAsync(userId, historyId)).ReturnsAsync(SettingsResult<UserSettingsDto>.Fail("Not found"));
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Restore(historyId);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
