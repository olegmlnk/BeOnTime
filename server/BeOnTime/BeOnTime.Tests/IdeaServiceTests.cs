using Moq;
using Xunit;
using BeOnTime.Application.Services;
using BeOnTime.Application.Interfaces;
using BeOnTime.Application.DTOs.Ideas;
using BeOnTime.Application.DTOs.Tasks;
using BeOnTime.Core.Entities;
using BeOnTime.Core.Enums;

namespace BeOnTime.Tests
{
    public class IdeaServiceTests
    {
        [Fact]
        public async Task GetAllAsync_ShouldReturnMappedIdeas()
        {
            // 1. ARRANGE
            var mockIdeaRepo = new Mock<IIdeaRepository>();
            var mockTaskService = new Mock<ITaskService>();
            var service = new IdeaService(mockIdeaRepo.Object, mockTaskService.Object);

            var userId = Guid.NewGuid();
            var ideas = new List<Idea>
            {
                new Idea { Id = Guid.NewGuid(), UserId = userId, Title = "Idea 1", Content = "Content 1" },
                new Idea { Id = Guid.NewGuid(), UserId = userId, Title = "Idea 2", Content = "Content 2" }
            };

            mockIdeaRepo
                .Setup(r => r.GetAllAsync(userId))
                .ReturnsAsync(ideas);

            // 2. ACT
            var result = await service.GetAllAsync(userId);

            // 3. ASSERT
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Idea 1", result[0].Title);
            Assert.Equal("Content 2", result[1].Content);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenIdeaDoesNotExist()
        {
            // 1. ARRANGE
            var mockIdeaRepo = new Mock<IIdeaRepository>();
            var mockTaskService = new Mock<ITaskService>();
            var service = new IdeaService(mockIdeaRepo.Object, mockTaskService.Object);

            var userId = Guid.NewGuid();

            mockIdeaRepo
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<Guid>()))
                .ReturnsAsync((Idea?)null);

            // 2. ACT
            var result = await service.GetByIdAsync(userId, Guid.NewGuid());

            // 3. ASSERT
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnMappedIdea_WhenIdeaExists()
        {
            // 1. ARRANGE
            var mockIdeaRepo = new Mock<IIdeaRepository>();
            var mockTaskService = new Mock<ITaskService>();
            var service = new IdeaService(mockIdeaRepo.Object, mockTaskService.Object);

            var userId = Guid.NewGuid();
            var ideaId = Guid.NewGuid();

            var idea = new Idea
            {
                Id = ideaId,
                UserId = userId,
                Title = "Test idea",
                Content = "Test content",
                IsConvertedToTask = false
            };

            mockIdeaRepo
                .Setup(r => r.GetByIdAsync(userId, ideaId))
                .ReturnsAsync(idea);

            // 2. ACT
            var result = await service.GetByIdAsync(userId, ideaId);

            // 3. ASSERT
            Assert.NotNull(result);
            Assert.Equal("Test idea", result.Title);
            Assert.Equal("Test content", result.Content);
            Assert.False(result.IsConvertedToTask);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnMappedIdea()
        {
            // 1. ARRANGE
            var mockIdeaRepo = new Mock<IIdeaRepository>();
            var mockTaskService = new Mock<ITaskService>();
            var service = new IdeaService(mockIdeaRepo.Object, mockTaskService.Object);

            var userId = Guid.NewGuid();
            var dto = new CreateIdeaDto { Title = "New idea", Content = "Idea description" };

            // 2. ACT
            var result = await service.CreateAsync(userId, dto);

            // 3. ASSERT
            Assert.NotNull(result);
            Assert.Equal("New idea", result.Title);
            Assert.Equal("Idea description", result.Content);
            Assert.False(result.IsConvertedToTask);

            // Перевірка виклику CreateAsync в репозиторії
            mockIdeaRepo.Verify(r => r.CreateAsync(It.IsAny<Idea>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnNull_WhenIdeaDoesNotExist()
        {
            // 1. ARRANGE
            var mockIdeaRepo = new Mock<IIdeaRepository>();
            var mockTaskService = new Mock<ITaskService>();
            var service = new IdeaService(mockIdeaRepo.Object, mockTaskService.Object);

            var userId = Guid.NewGuid();
            var ideaId = Guid.NewGuid();

            mockIdeaRepo
                .Setup(r => r.GetByIdAsync(userId, ideaId))
                .ReturnsAsync((Idea?)null);

            var dto = new UpdateIdeaDto { Title = "New title", Content = "New content" };

            // 2. ACT
            var result = await service.UpdateAsync(userId, ideaId, dto);

            // 3. ASSERT
            Assert.Null(result);
            mockIdeaRepo.Verify(r => r.UpdateAsync(It.IsAny<Idea>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateAndReturnIdea_WhenIdeaExists()
        {
            // 1. ARRANGE
            var mockIdeaRepo = new Mock<IIdeaRepository>();
            var mockTaskService = new Mock<ITaskService>();
            var service = new IdeaService(mockIdeaRepo.Object, mockTaskService.Object);

            var userId = Guid.NewGuid();
            var ideaId = Guid.NewGuid();

            var existingIdea = new Idea
            {
                Id = ideaId,
                UserId = userId,
                Title = "Old title",
                Content = "Old content"
            };

            mockIdeaRepo
                .Setup(r => r.GetByIdAsync(userId, ideaId))
                .ReturnsAsync(existingIdea);

            var dto = new UpdateIdeaDto { Title = "Updated title", Content = "Updated content" };

            // 2. ACT
            var result = await service.UpdateAsync(userId, ideaId, dto);

            // 3. ASSERT
            Assert.NotNull(result);
            Assert.Equal("Updated title", existingIdea.Title);
            Assert.Equal("Updated content", existingIdea.Content);
            mockIdeaRepo.Verify(r => r.UpdateAsync(existingIdea), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenIdeaDoesNotExist()
        {
            // 1. ARRANGE
            var mockIdeaRepo = new Mock<IIdeaRepository>();
            var mockTaskService = new Mock<ITaskService>();
            var service = new IdeaService(mockIdeaRepo.Object, mockTaskService.Object);

            mockIdeaRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((Idea?)null);

            // 2. ACT
            var result = await service.DeleteAsync(Guid.NewGuid(), Guid.NewGuid());

            // 3. ASSERT
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldSetDeletedAtAndReturnTrue_WhenIdeaExists()
        {
            // 1. ARRANGE
            var mockIdeaRepo = new Mock<IIdeaRepository>();
            var mockTaskService = new Mock<ITaskService>();
            var service = new IdeaService(mockIdeaRepo.Object, mockTaskService.Object);

            var userId = Guid.NewGuid();
            var ideaId = Guid.NewGuid();

            var existingIdea = new Idea
            {
                Id = ideaId,
                UserId = userId,
                Title = "Idea to delete"
            };

            mockIdeaRepo
                .Setup(r => r.GetByIdAsync(userId, ideaId))
                .ReturnsAsync(existingIdea);

            // 2. ACT
            var result = await service.DeleteAsync(userId, ideaId);

            // 3. ASSERT
            Assert.True(result);
            Assert.NotNull(existingIdea.DeletedAt); // Soft-delete: DeletedAt проставлений
            mockIdeaRepo.Verify(r => r.UpdateAsync(existingIdea), Times.Once);
        }

        [Fact]
        public async Task ConvertToTaskAsync_ShouldReturnNull_WhenIdeaDoesNotExist()
        {
            // 1. ARRANGE
            var mockIdeaRepo = new Mock<IIdeaRepository>();
            var mockTaskService = new Mock<ITaskService>();
            var service = new IdeaService(mockIdeaRepo.Object, mockTaskService.Object);

            mockIdeaRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((Idea?)null);

            // 2. ACT
            var result = await service.ConvertToTaskAsync(Guid.NewGuid(), Guid.NewGuid());

            // 3. ASSERT
            Assert.Null(result);
        }

        [Fact]
        public async Task ConvertToTaskAsync_ShouldReturnFail_WhenAlreadyConverted()
        {
            // 1. ARRANGE
            var mockIdeaRepo = new Mock<IIdeaRepository>();
            var mockTaskService = new Mock<ITaskService>();
            var service = new IdeaService(mockIdeaRepo.Object, mockTaskService.Object);

            var userId = Guid.NewGuid();
            var ideaId = Guid.NewGuid();

            var idea = new Idea
            {
                Id = ideaId,
                UserId = userId,
                Title = "Already converted",
                Content = "Content",
                IsConvertedToTask = true // Вже конвертовано!
            };

            mockIdeaRepo
                .Setup(r => r.GetByIdAsync(userId, ideaId))
                .ReturnsAsync(idea);

            // 2. ACT
            var result = await service.ConvertToTaskAsync(userId, ideaId);

            // 3. ASSERT
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("Idea has already been converted to a task", result.Error);
        }

        [Fact]
        public async Task ConvertToTaskAsync_ShouldCreateTaskAndMarkIdea_WhenNotConverted()
        {
            // 1. ARRANGE
            var mockIdeaRepo = new Mock<IIdeaRepository>();
            var mockTaskService = new Mock<ITaskService>();
            var service = new IdeaService(mockIdeaRepo.Object, mockTaskService.Object);

            var userId = Guid.NewGuid();
            var ideaId = Guid.NewGuid();

            var idea = new Idea
            {
                Id = ideaId,
                UserId = userId,
                Title = "Cool idea",
                Content = "Idea description",
                IsConvertedToTask = false
            };

            var createdTask = new TaskResponseDto
            {
                Id = Guid.NewGuid(),
                Title = "Cool idea",
                Description = "Idea description"
            };

            mockIdeaRepo
                .Setup(r => r.GetByIdAsync(userId, ideaId))
                .ReturnsAsync(idea);

            // Мокаємо TaskService.CreateAsync
            mockTaskService
                .Setup(s => s.CreateAsync(userId, It.IsAny<CreateTaskDto>()))
                .ReturnsAsync(createdTask);

            // 2. ACT
            var result = await service.ConvertToTaskAsync(userId, ideaId);

            // 3. ASSERT
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Task);
            Assert.Equal("Cool idea", result.Task.Title);

            // Ідея позначена як конвертована
            Assert.True(idea.IsConvertedToTask);
            mockIdeaRepo.Verify(r => r.UpdateAsync(idea), Times.Once);
        }
    }
}
