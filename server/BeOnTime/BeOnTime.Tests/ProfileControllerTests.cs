using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;
using BeOnTime.API.Controllers;
using BeOnTime.Application.Interfaces;
using BeOnTime.Application.DTOs.Profile;

namespace BeOnTime.Tests
{
    public class ProfileControllerTests
    {
        private ProfileController CreateControllerWithUser(Mock<IProfileService> mockService, Guid userId)
        {
            var controller = new ProfileController(mockService.Object);
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
            return controller;
        }

        [Fact]
        public async Task Get_ShouldReturnOk_WhenProfileExists()
        {
            var mockService = new Mock<IProfileService>();
            var userId = Guid.NewGuid();
            var profile = new ProfileDto { Id = userId, UserName = "testuser", Email = "test@test.com" };
            mockService.Setup(s => s.GetAsync(userId)).ReturnsAsync(profile);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<ProfileDto>(okResult.Value);
            Assert.Equal("testuser", returned.UserName);
        }

        [Fact]
        public async Task Get_ShouldReturnNotFound_WhenProfileDoesNotExist()
        {
            var mockService = new Mock<IProfileService>();
            var userId = Guid.NewGuid();
            mockService.Setup(s => s.GetAsync(userId)).ReturnsAsync((ProfileDto?)null);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Get();

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateUsername_ShouldReturnOk_WhenSucceeds()
        {
            var mockService = new Mock<IProfileService>();
            var userId = Guid.NewGuid();
            var dto = new UpdateUsernameDto { UserName = "newname" };
            var profile = new ProfileDto { Id = userId, UserName = "newname" };
            mockService.Setup(s => s.UpdateUsernameAsync(userId, dto)).ReturnsAsync(ProfileResult<ProfileDto>.Ok(profile));
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.UpdateUsername(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<ProfileDto>(okResult.Value);
            Assert.Equal("newname", returned.UserName);
        }

        [Fact]
        public async Task UpdateUsername_ShouldReturnBadRequest_WhenFails()
        {
            var mockService = new Mock<IProfileService>();
            var userId = Guid.NewGuid();
            var dto = new UpdateUsernameDto { UserName = "taken" };
            mockService.Setup(s => s.UpdateUsernameAsync(userId, dto)).ReturnsAsync(ProfileResult<ProfileDto>.Fail("Username taken"));
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.UpdateUsername(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateEmail_ShouldReturnOk_WhenSucceeds()
        {
            var mockService = new Mock<IProfileService>();
            var userId = Guid.NewGuid();
            var dto = new UpdateEmailDto { Email = "new@test.com", CurrentPassword = "pass" };
            var profile = new ProfileDto { Id = userId, Email = "new@test.com" };
            mockService.Setup(s => s.UpdateEmailAsync(userId, dto)).ReturnsAsync(ProfileResult<ProfileDto>.Ok(profile));
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.UpdateEmail(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<ProfileDto>(okResult.Value);
            Assert.Equal("new@test.com", returned.Email);
        }

        [Fact]
        public async Task UpdateEmail_ShouldReturnBadRequest_WhenFails()
        {
            var mockService = new Mock<IProfileService>();
            var userId = Guid.NewGuid();
            var dto = new UpdateEmailDto { Email = "taken@test.com", CurrentPassword = "pass" };
            mockService.Setup(s => s.UpdateEmailAsync(userId, dto)).ReturnsAsync(ProfileResult<ProfileDto>.Fail("Email taken"));
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.UpdateEmail(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnOk_WhenSucceeds()
        {
            var mockService = new Mock<IProfileService>();
            var userId = Guid.NewGuid();
            var dto = new ChangePasswordDto { CurrentPassword = "old", NewPassword = "new12345", ConfirmNewPassword = "new12345" };
            var profile = new ProfileDto { Id = userId };
            mockService.Setup(s => s.ChangePasswordAsync(userId, dto)).ReturnsAsync(ProfileResult<ProfileDto>.Ok(profile));
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.ChangePassword(dto);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnBadRequest_WhenFails()
        {
            var mockService = new Mock<IProfileService>();
            var userId = Guid.NewGuid();
            var dto = new ChangePasswordDto { CurrentPassword = "wrong", NewPassword = "new12345", ConfirmNewPassword = "new12345" };
            mockService.Setup(s => s.ChangePasswordAsync(userId, dto)).ReturnsAsync(ProfileResult<ProfileDto>.Fail("Wrong password"));
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.ChangePassword(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteAccount_ShouldReturnNoContent_WhenSucceeds()
        {
            var mockService = new Mock<IProfileService>();
            var userId = Guid.NewGuid();
            var dto = new DeleteAccountDto { CurrentPassword = "valid" };
            var profile = new ProfileDto { Id = userId };
            mockService.Setup(s => s.DeleteAccountAsync(userId, dto)).ReturnsAsync(ProfileResult<ProfileDto>.Ok(profile));
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.DeleteAccount(dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAccount_ShouldReturnBadRequest_WhenFails()
        {
            var mockService = new Mock<IProfileService>();
            var userId = Guid.NewGuid();
            var dto = new DeleteAccountDto { CurrentPassword = "wrong" };
            mockService.Setup(s => s.DeleteAccountAsync(userId, dto)).ReturnsAsync(ProfileResult<ProfileDto>.Fail("Wrong password"));
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.DeleteAccount(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
