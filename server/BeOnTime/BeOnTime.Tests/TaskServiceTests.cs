using Moq;
using Xunit;
using BeOnTime.Application.Services;
using BeOnTime.Application.Interfaces;
using BeOnTime.Application.DTOs.Tasks;
using BeOnTime.Core.Entities;
using BeOnTime.Core.Enums;
using System;
using System.Threading.Tasks;

namespace BeOnTime.Tests
{
    public class TaskServiceTests
    {
        [Fact]
        public async Task CreateAsync_ShouldReturnTaskResponseDto_WhenDataIsValid()
        {
            // 1. ARRANGE
            var mockTaskRepository = new Mock<ITaskRepository>();
            
            var taskService = new TaskService(mockTaskRepository.Object);

            var userId = Guid.NewGuid();
            
            // Створюємо запит на створення завдання
            var request = new CreateTaskDto
            {
                Title = "Написати тести для TaskService",
                Description = "Перевірити метод CreateAsync",
                // Пріоритет за замовчуванням у DTO - Medium
                Priority = TaskPriority.High 
            };

            // 2. ACT 
            var result = await taskService.CreateAsync(userId, request);

            // 3. ASSERT 
            Assert.NotNull(result);
            
            Assert.Equal(request.Title, result.Title);
            Assert.Equal(request.Description, result.Description);
            
            //Перевірка статусу за замовчуванням (у сервісі прописано TaskItemStatus.Todo)
            Assert.Equal(TaskItemStatus.Todo, result.Status); 

            //Перевірка виклику методу CreateAsync у репозиторії рівно 1 раз!
            mockTaskRepository.Verify(repo => repo.CreateAsync(It.IsAny<TaskItem>()), Times.Once); 
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenTaskDoesNotExist()
        {
            // 1. ARRANGE
            var mockTaskRepository = new Mock<ITaskRepository>();
            var taskService = new TaskService(mockTaskRepository.Object);

            var userId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            //Імітація ситуації коли в бд такого завдання не існує
            mockTaskRepository
                .Setup(repo => repo.GetByIdAsync(userId, taskId))
                .ReturnsAsync((TaskItem?)null);

            // 2. ACT
            var result = await taskService.DeleteAsync(userId, taskId);

            // 3. ASSERT
            Assert.False(result); 
            
            // Перевірка, що сервіс не викликав метод оновлення в БД
            mockTaskRepository.Verify(repo => repo.UpdateAsync(It.IsAny<TaskItem>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenTaskExists()
        {
            // 1. ARRANGE
            var mockTaskRepository = new Mock<ITaskRepository>();
            var taskService = new TaskService(mockTaskRepository.Object);

            var userId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            //Створення фейкового завдання
            var existingTask = new TaskItem
            {
                Id = taskId,
                UserId = userId,
                Title = "Завдання на видалення"
            };

            //Налаштування БД, щоб вона знаходила це завдання
            mockTaskRepository
                .Setup(repo => repo.GetByIdAsync(userId, taskId))
                .ReturnsAsync(existingTask);

            // 2. ACT
            var result = await taskService.DeleteAsync(userId, taskId);

            // 3. ASSERT
            Assert.True(result); // Має повернути true
            
            // Перевірка, чи був проставлений час видалення
            Assert.NotNull(existingTask.DeletedAt);
            
            // Перевірка, чи сервіс передав команду БД на збереження цих змін
            mockTaskRepository.Verify(repo => repo.UpdateAsync(existingTask), Times.Once); //
        }
    }
}