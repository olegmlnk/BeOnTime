using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;
using BeOnTime.API.Controllers;
using BeOnTime.Application.Interfaces;
using BeOnTime.Application.DTOs.Ideas;
using BeOnTime.Application.DTOs.Tasks;

namespace BeOnTime.Tests
{
    public class IdeasControllerTests
    {
        private IdeasController CreateControllerWithUser(Mock<IIdeaService> mockService, Guid userId)
        {
            var controller = new IdeasController(mockService.Object);
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
            return controller;
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithIdeas()
        {
            var mockService = new Mock<IIdeaService>();
            var userId = Guid.NewGuid();
            var ideas = new List<IdeaResponseDto>
            {
                new IdeaResponseDto { Id = Guid.NewGuid(), Title = "Idea 1" },
                new IdeaResponseDto { Id = Guid.NewGuid(), Title = "Idea 2" }
            };
            mockService.Setup(s => s.GetAllAsync(userId)).ReturnsAsync(ideas);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<IReadOnlyList<IdeaResponseDto>>(okResult.Value);
            Assert.Equal(2, returned.Count);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenIdeaExists()
        {
            var mockService = new Mock<IIdeaService>();
            var userId = Guid.NewGuid();
            var ideaId = Guid.NewGuid();
            var idea = new IdeaResponseDto { Id = ideaId, Title = "Test Idea" };
            mockService.Setup(s => s.GetByIdAsync(userId, ideaId)).ReturnsAsync(idea);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.GetById(ideaId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<IdeaResponseDto>(okResult.Value);
            Assert.Equal("Test Idea", returned.Title);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenIdeaDoesNotExist()
        {
            var mockService = new Mock<IIdeaService>();
            var userId = Guid.NewGuid();
            var ideaId = Guid.NewGuid();
            mockService.Setup(s => s.GetByIdAsync(userId, ideaId)).ReturnsAsync((IdeaResponseDto?)null);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.GetById(ideaId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction()
        {
            var mockService = new Mock<IIdeaService>();
            var userId = Guid.NewGuid();
            var ideaId = Guid.NewGuid();
            var dto = new CreateIdeaDto { Title = "New Idea", Content = "Content" };
            var created = new IdeaResponseDto { Id = ideaId, Title = "New Idea", Content = "Content" };
            mockService.Setup(s => s.CreateAsync(userId, dto)).ReturnsAsync(created);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Create(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returned = Assert.IsType<IdeaResponseDto>(createdResult.Value);
            Assert.Equal(ideaId, returned.Id);
        }

        [Fact]
        public async Task Update_ShouldReturnOk_WhenIdeaExists()
        {
            var mockService = new Mock<IIdeaService>();
            var userId = Guid.NewGuid();
            var ideaId = Guid.NewGuid();
            var dto = new UpdateIdeaDto { Title = "Updated", Content = "Updated content" };
            var updated = new IdeaResponseDto { Id = ideaId, Title = "Updated" };
            mockService.Setup(s => s.UpdateAsync(userId, ideaId, dto)).ReturnsAsync(updated);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Update(ideaId, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<IdeaResponseDto>(okResult.Value);
            Assert.Equal("Updated", returned.Title);
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenIdeaDoesNotExist()
        {
            var mockService = new Mock<IIdeaService>();
            var userId = Guid.NewGuid();
            var ideaId = Guid.NewGuid();
            var dto = new UpdateIdeaDto { Title = "Updated", Content = "Content" };
            mockService.Setup(s => s.UpdateAsync(userId, ideaId, dto)).ReturnsAsync((IdeaResponseDto?)null);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Update(ideaId, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenDeleteSucceeds()
        {
            var mockService = new Mock<IIdeaService>();
            var userId = Guid.NewGuid();
            var ideaId = Guid.NewGuid();
            mockService.Setup(s => s.DeleteAsync(userId, ideaId)).ReturnsAsync(true);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Delete(ideaId);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenDeleteFails()
        {
            var mockService = new Mock<IIdeaService>();
            var userId = Guid.NewGuid();
            var ideaId = Guid.NewGuid();
            mockService.Setup(s => s.DeleteAsync(userId, ideaId)).ReturnsAsync(false);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Delete(ideaId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ConvertToTask_ShouldReturnOk_WhenSucceeds()
        {
            var mockService = new Mock<IIdeaService>();
            var userId = Guid.NewGuid();
            var ideaId = Guid.NewGuid();
            var taskDto = new TaskResponseDto { Id = Guid.NewGuid(), Title = "Converted" };
            mockService.Setup(s => s.ConvertToTaskAsync(userId, ideaId)).ReturnsAsync(ConvertIdeaResult.Ok(taskDto));
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.ConvertToTask(ideaId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<TaskResponseDto>(okResult.Value);
        }

        [Fact]
        public async Task ConvertToTask_ShouldReturnNotFound_WhenIdeaNotFound()
        {
            var mockService = new Mock<IIdeaService>();
            var userId = Guid.NewGuid();
            var ideaId = Guid.NewGuid();
            mockService.Setup(s => s.ConvertToTaskAsync(userId, ideaId)).ReturnsAsync((ConvertIdeaResult?)null);
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.ConvertToTask(ideaId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ConvertToTask_ShouldReturnBadRequest_WhenConversionFails()
        {
            var mockService = new Mock<IIdeaService>();
            var userId = Guid.NewGuid();
            var ideaId = Guid.NewGuid();
            mockService.Setup(s => s.ConvertToTaskAsync(userId, ideaId)).ReturnsAsync(ConvertIdeaResult.Fail("Already converted"));
            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.ConvertToTask(ideaId);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
