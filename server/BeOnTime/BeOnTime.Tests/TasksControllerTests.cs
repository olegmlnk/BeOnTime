using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;
using BeOnTime.API.Controllers;
using BeOnTime.Application.Interfaces;
using BeOnTime.Application.DTOs.Tasks;

namespace BeOnTime.Tests
{
    public class TasksControllerTests
    {
        // Допоміжний метод для створення контролера з "залогіненим" користувачем
        private TasksController CreateControllerWithUser(Mock<ITaskService> mockService, Guid userId)
        {
            var controller = new TasksController(mockService.Object);

            // Створення фейкових даних (Claims) користувача, ніби він передав валідний токен
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Передача  цього фейкового юзера в контекст контролера
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            return controller;
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenTaskDoesNotExist()
        {
            // 1. ARRANGE
            var mockTaskService = new Mock<ITaskService>();
            var userId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            // Сервіс повертає null (таски немає)
            mockTaskService
                .Setup(s => s.GetByIdAsync(userId, taskId))
                .ReturnsAsync((TaskResponseDto?)null);

            var controller = CreateControllerWithUser(mockTaskService, userId);

            // 2. ACT
            var result = await controller.GetById(taskId);

            // 3. ASSERT
            // Перевірка, чи контролер справді повернув статус 404 Not Found
            Assert.IsType<NotFoundResult>(result); //
        }

        [Fact]
        public async Task GetById_ShouldReturnOkWithTask_WhenTaskExists()
        {
            // 1. ARRANGE
            var mockTaskService = new Mock<ITaskService>();
            var userId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            var expectedTask = new TaskResponseDto { Id = taskId, Title = "Тестова таска" };

            // Сервіс знаходить таску і повертає її
            mockTaskService
                .Setup(s => s.GetByIdAsync(userId, taskId))
                .ReturnsAsync(expectedTask);

            var controller = CreateControllerWithUser(mockTaskService, userId);

            // 2. ACT
            var result = await controller.GetById(taskId);

            // 3. ASSERT
            // Перевірка, чи повернувся статус 200 OK
            var okResult = Assert.IsType<OkObjectResult>(result); 
            
            // Перевірка, чи всередині 200 OK лежить саме наша таска
            var returnedTask = Assert.IsType<TaskResponseDto>(okResult.Value);
            Assert.Equal("Тестова таска", returnedTask.Title);
        }
    }
}