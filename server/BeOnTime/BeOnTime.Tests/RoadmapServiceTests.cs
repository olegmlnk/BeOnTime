using Moq;
using Xunit;
using BeOnTime.Application.Services;
using BeOnTime.Application.Interfaces;
using BeOnTime.Application.DTOs.Roadmaps;
using BeOnTime.Core.Entities;
using BeOnTime.Core.Enums;

namespace BeOnTime.Tests
{
    public class RoadmapServiceTests
    {
        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenRoadmapDoesNotExist()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IRoadmapRepository>();
            var service = new RoadmapService(mockRepo.Object);

            var userId = Guid.NewGuid();

            mockRepo
                .Setup(r => r.GetWithTasksAsync(userId, It.IsAny<Guid>()))
                .ReturnsAsync((Roadmap?)null);

            // 2. ACT
            var result = await service.GetByIdAsync(userId, Guid.NewGuid());

            // 3. ASSERT
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnMappedDetails_WhenRoadmapExists()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IRoadmapRepository>();
            var service = new RoadmapService(mockRepo.Object);

            var userId = Guid.NewGuid();
            var roadmapId = Guid.NewGuid();

            var roadmap = new Roadmap
            {
                Id = roadmapId,
                UserId = userId,
                Name = "My roadmap",
                Description = "Description",
                Tasks = new List<TaskItem>
                {
                    new TaskItem { Id = Guid.NewGuid(), Status = TaskItemStatus.Done },
                    new TaskItem { Id = Guid.NewGuid(), Status = TaskItemStatus.Todo }
                }
            };

            mockRepo
                .Setup(r => r.GetWithTasksAsync(userId, roadmapId))
                .ReturnsAsync(roadmap);

            // 2. ACT
            var result = await service.GetByIdAsync(userId, roadmapId);

            // 3. ASSERT
            Assert.NotNull(result);
            Assert.Equal("My roadmap", result.Name);
            Assert.Equal(2, result.TotalTasks);
            Assert.Equal(1, result.DoneTasks);
            Assert.Equal(50, result.ProgressPercent); // 1 з 2 = 50%
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnSuccess_WhenDatesAreValid()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IRoadmapRepository>();
            var service = new RoadmapService(mockRepo.Object);

            var userId = Guid.NewGuid();
            var dto = new CreateRoadmapDto
            {
                Name = "New roadmap",
                Description = "Description",
                StartDate = DateTime.UtcNow,
                TargetDate = DateTime.UtcNow.AddDays(30)
            };

            // 2. ACT
            var result = await service.CreateAsync(userId, dto);

            // 3. ASSERT
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.Equal("New roadmap", result.Value.Name);
            mockRepo.Verify(r => r.CreateAsync(It.IsAny<Roadmap>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnFail_WhenStartDateIsAfterTargetDate()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IRoadmapRepository>();
            var service = new RoadmapService(mockRepo.Object);

            var userId = Guid.NewGuid();
            var dto = new CreateRoadmapDto
            {
                Name = "Invalid roadmap",
                StartDate = DateTime.UtcNow.AddDays(30), // Старт ПІСЛЯ дедлайну
                TargetDate = DateTime.UtcNow
            };

            // 2. ACT
            var result = await service.CreateAsync(userId, dto);

            // 3. ASSERT
            Assert.False(result.Success);
            Assert.Equal("StartDate must be on or before TargetDate", result.Error);
            mockRepo.Verify(r => r.CreateAsync(It.IsAny<Roadmap>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ShouldSucceed_WhenDatesAreNull()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IRoadmapRepository>();
            var service = new RoadmapService(mockRepo.Object);

            var userId = Guid.NewGuid();
            var dto = new CreateRoadmapDto
            {
                Name = "Roadmap without dates",
                StartDate = null,
                TargetDate = null
            };

            // 2. ACT
            var result = await service.CreateAsync(userId, dto);

            // 3. ASSERT
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnNull_WhenRoadmapDoesNotExist()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IRoadmapRepository>();
            var service = new RoadmapService(mockRepo.Object);

            var userId = Guid.NewGuid();

            mockRepo
                .Setup(r => r.GetWithTasksAsync(userId, It.IsAny<Guid>()))
                .ReturnsAsync((Roadmap?)null);

            var dto = new UpdateRoadmapDto { Name = "New name" };

            // 2. ACT
            var result = await service.UpdateAsync(userId, Guid.NewGuid(), dto);

            // 3. ASSERT
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFail_WhenDatesAreInvalid()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IRoadmapRepository>();
            var service = new RoadmapService(mockRepo.Object);

            var userId = Guid.NewGuid();
            var roadmapId = Guid.NewGuid();

            var roadmap = new Roadmap
            {
                Id = roadmapId,
                UserId = userId,
                Name = "Existing roadmap",
                Tasks = new List<TaskItem>()
            };

            mockRepo
                .Setup(r => r.GetWithTasksAsync(userId, roadmapId))
                .ReturnsAsync(roadmap);

            // StartDate > TargetDate — невалідно
            var dto = new UpdateRoadmapDto
            {
                Name = "Updated name",
                StartDate = DateTime.UtcNow.AddDays(10),
                TargetDate = DateTime.UtcNow
            };

            // 2. ACT
            var result = await service.UpdateAsync(userId, roadmapId, dto);

            // 3. ASSERT
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("StartDate must be on or before TargetDate", result.Error);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenDataIsValid()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IRoadmapRepository>();
            var service = new RoadmapService(mockRepo.Object);

            var userId = Guid.NewGuid();
            var roadmapId = Guid.NewGuid();

            var roadmap = new Roadmap
            {
                Id = roadmapId,
                UserId = userId,
                Name = "Old name",
                Tasks = new List<TaskItem>()
            };

            mockRepo
                .Setup(r => r.GetWithTasksAsync(userId, roadmapId))
                .ReturnsAsync(roadmap);

            var dto = new UpdateRoadmapDto
            {
                Name = "Updated name",
                Description = "New description",
                StartDate = DateTime.UtcNow,
                TargetDate = DateTime.UtcNow.AddDays(30)
            };

            // 2. ACT
            var result = await service.UpdateAsync(userId, roadmapId, dto);

            // 3. ASSERT
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Updated name", roadmap.Name);
            Assert.Equal("New description", roadmap.Description);
            mockRepo.Verify(r => r.UpdateAsync(roadmap), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenRoadmapDoesNotExist()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IRoadmapRepository>();
            var service = new RoadmapService(mockRepo.Object);

            var userId = Guid.NewGuid();

            mockRepo
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<Guid>()))
                .ReturnsAsync((Roadmap?)null);

            // 2. ACT
            var result = await service.DeleteAsync(userId, Guid.NewGuid());

            // 3. ASSERT
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldSoftDeleteAndUnlinkTasks_WhenRoadmapExists()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IRoadmapRepository>();
            var service = new RoadmapService(mockRepo.Object);

            var userId = Guid.NewGuid();
            var roadmapId = Guid.NewGuid();

            var roadmap = new Roadmap
            {
                Id = roadmapId,
                UserId = userId,
                Name = "Roadmap to delete"
            };

            // Таски, прив'язані до роадмапу
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = Guid.NewGuid(), RoadmapId = roadmapId, OrderInRoadmap = 0 },
                new TaskItem { Id = Guid.NewGuid(), RoadmapId = roadmapId, OrderInRoadmap = 1 }
            };

            mockRepo
                .Setup(r => r.GetByIdAsync(userId, roadmapId))
                .ReturnsAsync(roadmap);
            mockRepo
                .Setup(r => r.GetRoadmapTasksAsync(userId, roadmapId))
                .ReturnsAsync(tasks);

            // 2. ACT
            var result = await service.DeleteAsync(userId, roadmapId);

            // 3. ASSERT
            Assert.True(result);

            // Роадмап soft-deleted
            Assert.NotNull(roadmap.DeletedAt);

            // Таски від'єднані від роадмапу
            Assert.All(tasks, t =>
            {
                Assert.Null(t.RoadmapId);
                Assert.Null(t.OrderInRoadmap);
            });

            mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ReorderAsync_ShouldReturnNull_WhenRoadmapDoesNotExist()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IRoadmapRepository>();
            var service = new RoadmapService(mockRepo.Object);

            var userId = Guid.NewGuid();

            mockRepo
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<Guid>()))
                .ReturnsAsync((Roadmap?)null);

            var dto = new ReorderRoadmapDto { TaskIds = new List<Guid>() };

            // 2. ACT
            var result = await service.ReorderAsync(userId, Guid.NewGuid(), dto);

            // 3. ASSERT
            Assert.Null(result);
        }

        [Fact]
        public async Task ReorderAsync_ShouldReturnFail_WhenTaskIdsDoNotMatch()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IRoadmapRepository>();
            var service = new RoadmapService(mockRepo.Object);

            var userId = Guid.NewGuid();
            var roadmapId = Guid.NewGuid();

            var roadmap = new Roadmap { Id = roadmapId, UserId = userId };

            var task1 = new TaskItem { Id = Guid.NewGuid() };
            var task2 = new TaskItem { Id = Guid.NewGuid() };

            mockRepo
                .Setup(r => r.GetByIdAsync(userId, roadmapId))
                .ReturnsAsync(roadmap);
            mockRepo
                .Setup(r => r.GetRoadmapTasksAsync(userId, roadmapId))
                .ReturnsAsync(new List<TaskItem> { task1, task2 });

            // Передаємо тільки 1 з 2 тасків — невалідно
            var dto = new ReorderRoadmapDto { TaskIds = new List<Guid> { task1.Id } };

            // 2. ACT
            var result = await service.ReorderAsync(userId, roadmapId, dto);

            // 3. ASSERT
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Contains("TaskIds must exactly match", result.Error);
        }

        [Fact]
        public async Task ReorderAsync_ShouldUpdateOrder_WhenTaskIdsAreValid()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IRoadmapRepository>();
            var service = new RoadmapService(mockRepo.Object);

            var userId = Guid.NewGuid();
            var roadmapId = Guid.NewGuid();

            var roadmap = new Roadmap
            {
                Id = roadmapId,
                UserId = userId,
                Name = "Roadmap"
            };

            var task1 = new TaskItem { Id = Guid.NewGuid(), OrderInRoadmap = 0 };
            var task2 = new TaskItem { Id = Guid.NewGuid(), OrderInRoadmap = 1 };

            mockRepo
                .Setup(r => r.GetByIdAsync(userId, roadmapId))
                .ReturnsAsync(roadmap);
            mockRepo
                .Setup(r => r.GetRoadmapTasksAsync(userId, roadmapId))
                .ReturnsAsync(new List<TaskItem> { task1, task2 });

            // Повертаємо оновлений роадмап для фінального маппінгу
            mockRepo
                .Setup(r => r.GetWithTasksAsync(userId, roadmapId))
                .ReturnsAsync(new Roadmap
                {
                    Id = roadmapId,
                    UserId = userId,
                    Name = "Roadmap",
                    Tasks = new List<TaskItem> { task2, task1 }
                });

            // Міняємо порядок: task2 перший, task1 другий
            var dto = new ReorderRoadmapDto { TaskIds = new List<Guid> { task2.Id, task1.Id } };

            // 2. ACT
            var result = await service.ReorderAsync(userId, roadmapId, dto);

            // 3. ASSERT
            Assert.NotNull(result);
            Assert.True(result.Success);

            // Перевірка нового порядку
            Assert.Equal(0, task2.OrderInRoadmap);
            Assert.Equal(1, task1.OrderInRoadmap);

            mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturn0Progress_WhenNoTasks()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IRoadmapRepository>();
            var service = new RoadmapService(mockRepo.Object);

            var userId = Guid.NewGuid();
            var roadmapId = Guid.NewGuid();

            var roadmap = new Roadmap
            {
                Id = roadmapId,
                UserId = userId,
                Name = "Empty roadmap",
                Tasks = new List<TaskItem>() // Нуль тасків
            };

            mockRepo
                .Setup(r => r.GetWithTasksAsync(userId, roadmapId))
                .ReturnsAsync(roadmap);

            // 2. ACT
            var result = await service.GetByIdAsync(userId, roadmapId);

            // 3. ASSERT
            Assert.NotNull(result);
            Assert.Equal(0, result.ProgressPercent); // Ділення на 0 не повинно впасти
            Assert.Equal(0, result.TotalTasks);
        }
    }
}
