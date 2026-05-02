using Moq;
using Xunit;
using BeOnTime.Application.Services;
using BeOnTime.Application.Interfaces;
using BeOnTime.Application.DTOs.Archive;
using BeOnTime.Core.Entities;

namespace BeOnTime.Tests
{
    public class ArchiveServiceTests
    {
        [Fact]
        public async Task GetAllAsync_ShouldDelegateToRepository()
        {
            // 1. ARRANGE
            var mockArchive = new Mock<IArchiveRepository>();
            var service = new ArchiveService(mockArchive.Object);

            var userId = Guid.NewGuid();
            var expected = new List<ArchiveItemDto>
            {
                new ArchiveItemDto { Id = Guid.NewGuid(), Type = "task", Title = "Deleted task" }
            };

            mockArchive
                .Setup(r => r.GetAllAsync(userId))
                .ReturnsAsync(expected);

            // 2. ACT
            var result = await service.GetAllAsync(userId);

            // 3. ASSERT
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Deleted task", result[0].Title);
        }

        [Fact]
        public async Task RestoreAsync_ShouldReturnTrue_WhenDeletedTaskExists()
        {
            // 1. ARRANGE
            var mockArchive = new Mock<IArchiveRepository>();
            var service = new ArchiveService(mockArchive.Object);

            var userId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            var deletedTask = new TaskItem
            {
                Id = taskId,
                UserId = userId,
                Title = "Deleted task",
                DeletedAt = DateTime.UtcNow.AddDays(-1)
            };

            mockArchive
                .Setup(r => r.GetDeletedTaskAsync(userId, taskId))
                .ReturnsAsync(deletedTask);

            // 2. ACT
            var result = await service.RestoreAsync(userId, "task", taskId);

            // 3. ASSERT
            Assert.True(result);

            // Перевірка, що DeletedAt скинувся на null (відновлення)
            Assert.Null(deletedTask.DeletedAt);

            // Перевірка, що UpdatedAt оновився
            Assert.NotEqual(default, deletedTask.UpdatedAt);

            // Перевірка, що зміни збережені до БД
            mockArchive.Verify(r => r.UpdateTaskAsync(deletedTask), Times.Once);
        }

        [Fact]
        public async Task RestoreAsync_ShouldReturnFalse_WhenDeletedTaskNotFound()
        {
            // 1. ARRANGE
            var mockArchive = new Mock<IArchiveRepository>();
            var service = new ArchiveService(mockArchive.Object);

            var userId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            mockArchive
                .Setup(r => r.GetDeletedTaskAsync(userId, taskId))
                .ReturnsAsync((TaskItem?)null);

            // 2. ACT
            var result = await service.RestoreAsync(userId, "task", taskId);

            // 3. ASSERT
            Assert.False(result);
            mockArchive.Verify(r => r.UpdateTaskAsync(It.IsAny<TaskItem>()), Times.Never);
        }

        [Fact]
        public async Task RestoreAsync_ShouldReturnTrue_WhenDeletedIdeaExists()
        {
            // 1. ARRANGE
            var mockArchive = new Mock<IArchiveRepository>();
            var service = new ArchiveService(mockArchive.Object);

            var userId = Guid.NewGuid();
            var ideaId = Guid.NewGuid();

            var deletedIdea = new Idea
            {
                Id = ideaId,
                UserId = userId,
                Title = "Deleted idea",
                DeletedAt = DateTime.UtcNow.AddDays(-2)
            };

            mockArchive
                .Setup(r => r.GetDeletedIdeaAsync(userId, ideaId))
                .ReturnsAsync(deletedIdea);

            // 2. ACT
            var result = await service.RestoreAsync(userId, "idea", ideaId);

            // 3. ASSERT
            Assert.True(result);
            Assert.Null(deletedIdea.DeletedAt);
            mockArchive.Verify(r => r.UpdateIdeaAsync(deletedIdea), Times.Once);
        }

        [Fact]
        public async Task RestoreAsync_ShouldReturnFalse_WhenDeletedIdeaNotFound()
        {
            // 1. ARRANGE
            var mockArchive = new Mock<IArchiveRepository>();
            var service = new ArchiveService(mockArchive.Object);

            var userId = Guid.NewGuid();

            mockArchive
                .Setup(r => r.GetDeletedIdeaAsync(userId, It.IsAny<Guid>()))
                .ReturnsAsync((Idea?)null);

            // 2. ACT
            var result = await service.RestoreAsync(userId, "idea", Guid.NewGuid());

            // 3. ASSERT
            Assert.False(result);
        }

        [Fact]
        public async Task RestoreAsync_ShouldReturnTrue_WhenDeletedRoadmapExists()
        {
            // 1. ARRANGE
            var mockArchive = new Mock<IArchiveRepository>();
            var service = new ArchiveService(mockArchive.Object);

            var userId = Guid.NewGuid();
            var roadmapId = Guid.NewGuid();

            var deletedRoadmap = new Roadmap
            {
                Id = roadmapId,
                UserId = userId,
                Name = "Deleted roadmap",
                DeletedAt = DateTime.UtcNow.AddDays(-3)
            };

            mockArchive
                .Setup(r => r.GetDeletedRoadmapAsync(userId, roadmapId))
                .ReturnsAsync(deletedRoadmap);

            // 2. ACT
            var result = await service.RestoreAsync(userId, "roadmap", roadmapId);

            // 3. ASSERT
            Assert.True(result);
            Assert.Null(deletedRoadmap.DeletedAt);
            mockArchive.Verify(r => r.UpdateRoadmapAsync(deletedRoadmap), Times.Once);
        }

        [Fact]
        public async Task RestoreAsync_ShouldReturnFalse_WhenTypeIsUnknown()
        {
            // 1. ARRANGE
            var mockArchive = new Mock<IArchiveRepository>();
            var service = new ArchiveService(mockArchive.Object);

            // 2. ACT
            var result = await service.RestoreAsync(Guid.NewGuid(), "unknown", Guid.NewGuid());

            // 3. ASSERT
            Assert.False(result);
        }

        [Fact]
        public async Task RestoreAsync_ShouldHandleCaseInsensitiveType()
        {
            // 1. ARRANGE
            var mockArchive = new Mock<IArchiveRepository>();
            var service = new ArchiveService(mockArchive.Object);

            var userId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            var deletedTask = new TaskItem
            {
                Id = taskId,
                UserId = userId,
                Title = "Test task",
                DeletedAt = DateTime.UtcNow.AddDays(-1)
            };

            mockArchive
                .Setup(r => r.GetDeletedTaskAsync(userId, taskId))
                .ReturnsAsync(deletedTask);

            // 2. ACT — передаємо "TASK" великими літерами
            var result = await service.RestoreAsync(userId, "TASK", taskId);

            // 3. ASSERT — ToLowerInvariant() має це опрацювати
            Assert.True(result);
            Assert.Null(deletedTask.DeletedAt);
        }
    }
}
