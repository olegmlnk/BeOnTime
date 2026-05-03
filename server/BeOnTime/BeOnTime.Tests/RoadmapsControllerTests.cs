using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;
using BeOnTime.API.Controllers;
using BeOnTime.Application.Interfaces;
using BeOnTime.Application.DTOs.Roadmaps;

namespace BeOnTime.Tests
{
    public class RoadmapsControllerTests
    {
        private RoadmapsController CreateControllerWithUser(Mock<IRoadmapService> mockService, Guid userId)
        {
            var controller = new RoadmapsController(mockService.Object);
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
            return controller;
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithRoadmaps()
        {
            var mockService = new Mock<IRoadmapService>();
            var userId = Guid.NewGuid();
            var roadmaps = new List<RoadmapListItemDto>
            {
                new RoadmapListItemDto { Id = Guid.NewGuid(), Name = "Roadmap 1" }
            };
            mockService.Setup(s => s.GetAllAsync(userId)).ReturnsAsync(roadmaps);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<IReadOnlyList<RoadmapListItemDto>>(okResult.Value);
            Assert.Single(returned);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenRoadmapExists()
        {
            var mockService = new Mock<IRoadmapService>();
            var userId = Guid.NewGuid();
            var roadmapId = Guid.NewGuid();
            var roadmap = new RoadmapDetailsDto { Id = roadmapId, Name = "Test" };
            mockService.Setup(s => s.GetByIdAsync(userId, roadmapId)).ReturnsAsync(roadmap);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.GetById(roadmapId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<RoadmapDetailsDto>(okResult.Value);
            Assert.Equal("Test", returned.Name);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenRoadmapDoesNotExist()
        {
            var mockService = new Mock<IRoadmapService>();
            var userId = Guid.NewGuid();
            var roadmapId = Guid.NewGuid();
            mockService.Setup(s => s.GetByIdAsync(userId, roadmapId)).ReturnsAsync((RoadmapDetailsDto?)null);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.GetById(roadmapId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WhenSucceeds()
        {
            var mockService = new Mock<IRoadmapService>();
            var userId = Guid.NewGuid();
            var roadmapId = Guid.NewGuid();
            var dto = new CreateRoadmapDto { Name = "New Roadmap" };
            var details = new RoadmapDetailsDto { Id = roadmapId, Name = "New Roadmap" };
            mockService.Setup(s => s.CreateAsync(userId, dto)).ReturnsAsync(RoadmapResult<RoadmapDetailsDto>.Ok(details));
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Create(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var returned = Assert.IsType<RoadmapDetailsDto>(created.Value);
            Assert.Equal(roadmapId, returned.Id);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenFails()
        {
            var mockService = new Mock<IRoadmapService>();
            var userId = Guid.NewGuid();
            var dto = new CreateRoadmapDto { Name = "Bad" };
            mockService.Setup(s => s.CreateAsync(userId, dto)).ReturnsAsync(RoadmapResult<RoadmapDetailsDto>.Fail("Validation error"));
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Create(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Update_ShouldReturnOk_WhenSucceeds()
        {
            var mockService = new Mock<IRoadmapService>();
            var userId = Guid.NewGuid();
            var roadmapId = Guid.NewGuid();
            var dto = new UpdateRoadmapDto { Name = "Updated" };
            var details = new RoadmapDetailsDto { Id = roadmapId, Name = "Updated" };
            mockService.Setup(s => s.UpdateAsync(userId, roadmapId, dto)).ReturnsAsync(RoadmapResult<RoadmapDetailsDto>.Ok(details));
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Update(roadmapId, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<RoadmapDetailsDto>(okResult.Value);
            Assert.Equal("Updated", returned.Name);
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenRoadmapDoesNotExist()
        {
            var mockService = new Mock<IRoadmapService>();
            var userId = Guid.NewGuid();
            var roadmapId = Guid.NewGuid();
            var dto = new UpdateRoadmapDto { Name = "Updated" };
            mockService.Setup(s => s.UpdateAsync(userId, roadmapId, dto)).ReturnsAsync((RoadmapResult<RoadmapDetailsDto>?)null);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Update(roadmapId, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenValidationFails()
        {
            var mockService = new Mock<IRoadmapService>();
            var userId = Guid.NewGuid();
            var roadmapId = Guid.NewGuid();
            var dto = new UpdateRoadmapDto { Name = "Bad" };
            mockService.Setup(s => s.UpdateAsync(userId, roadmapId, dto)).ReturnsAsync(RoadmapResult<RoadmapDetailsDto>.Fail("Error"));
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Update(roadmapId, dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenSucceeds()
        {
            var mockService = new Mock<IRoadmapService>();
            var userId = Guid.NewGuid();
            var roadmapId = Guid.NewGuid();
            mockService.Setup(s => s.DeleteAsync(userId, roadmapId)).ReturnsAsync(true);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Delete(roadmapId);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenFails()
        {
            var mockService = new Mock<IRoadmapService>();
            var userId = Guid.NewGuid();
            var roadmapId = Guid.NewGuid();
            mockService.Setup(s => s.DeleteAsync(userId, roadmapId)).ReturnsAsync(false);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Delete(roadmapId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Reorder_ShouldReturnOk_WhenSucceeds()
        {
            var mockService = new Mock<IRoadmapService>();
            var userId = Guid.NewGuid();
            var roadmapId = Guid.NewGuid();
            var dto = new ReorderRoadmapDto { TaskIds = new List<Guid> { Guid.NewGuid() } };
            var details = new RoadmapDetailsDto { Id = roadmapId, Name = "Reordered" };
            mockService.Setup(s => s.ReorderAsync(userId, roadmapId, dto)).ReturnsAsync(RoadmapResult<RoadmapDetailsDto>.Ok(details));
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Reorder(roadmapId, dto);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Reorder_ShouldReturnNotFound_WhenRoadmapDoesNotExist()
        {
            var mockService = new Mock<IRoadmapService>();
            var userId = Guid.NewGuid();
            var roadmapId = Guid.NewGuid();
            var dto = new ReorderRoadmapDto { TaskIds = new List<Guid>() };
            mockService.Setup(s => s.ReorderAsync(userId, roadmapId, dto)).ReturnsAsync((RoadmapResult<RoadmapDetailsDto>?)null);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Reorder(roadmapId, dto);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
