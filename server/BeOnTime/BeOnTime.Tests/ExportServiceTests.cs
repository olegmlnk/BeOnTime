using Moq;
using Xunit;
using BeOnTime.Application.Services;
using BeOnTime.Application.Interfaces;
using BeOnTime.Core.Entities;
using BeOnTime.Core.Enums;

namespace BeOnTime.Tests
{
    public class ExportServiceTests
    {
        [Fact]
        public async Task EnqueueAsync_ShouldCreateJobAndReturnDto()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IUserExportJobRepository>();
            var service = new ExportService(mockRepo.Object);

            var userId = Guid.NewGuid();

            // 2. ACT
            var result = await service.EnqueueAsync(userId);

            // 3. ASSERT
            Assert.NotNull(result);
            Assert.Equal(ExportJobStatus.Pending, result.Status);
            Assert.NotEqual(Guid.Empty, result.Id);

            // Перевірка, що CreateAsync викликався з правильним статусом
            mockRepo.Verify(r => r.CreateAsync(It.Is<UserExportJob>(j =>
                j.UserId == userId &&
                j.Status == ExportJobStatus.Pending
            )), Times.Once);
        }

        [Fact]
        public async Task GetStatusAsync_ShouldReturnNull_WhenJobDoesNotExist()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IUserExportJobRepository>();
            var service = new ExportService(mockRepo.Object);

            var userId = Guid.NewGuid();

            mockRepo
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<Guid>()))
                .ReturnsAsync((UserExportJob?)null);

            // 2. ACT
            var result = await service.GetStatusAsync(userId, Guid.NewGuid());

            // 3. ASSERT
            Assert.Null(result);
        }

        [Fact]
        public async Task GetStatusAsync_ShouldReturnMappedDto_WhenJobExists()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IUserExportJobRepository>();
            var service = new ExportService(mockRepo.Object);

            var userId = Guid.NewGuid();
            var jobId = Guid.NewGuid();

            var job = new UserExportJob
            {
                Id = jobId,
                UserId = userId,
                Status = ExportJobStatus.Completed,
                FileName = "export.json",
                FileSizeBytes = 1024,
                CompletedAt = DateTime.UtcNow
            };

            mockRepo
                .Setup(r => r.GetByIdAsync(userId, jobId))
                .ReturnsAsync(job);

            // 2. ACT
            var result = await service.GetStatusAsync(userId, jobId);

            // 3. ASSERT
            Assert.NotNull(result);
            Assert.Equal(ExportJobStatus.Completed, result.Status);
            Assert.Equal("export.json", result.FileName);
            Assert.Equal(1024, result.FileSizeBytes);
        }

        [Fact]
        public async Task GetFileAsync_ShouldReturnNull_WhenJobDoesNotExist()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IUserExportJobRepository>();
            var service = new ExportService(mockRepo.Object);

            var userId = Guid.NewGuid();

            mockRepo
                .Setup(r => r.GetByIdWithContentAsync(userId, It.IsAny<Guid>()))
                .ReturnsAsync((UserExportJob?)null);

            // 2. ACT
            var result = await service.GetFileAsync(userId, Guid.NewGuid());

            // 3. ASSERT
            Assert.Null(result);
        }

        [Fact]
        public async Task GetFileAsync_ShouldReturnNull_WhenJobIsNotCompleted()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IUserExportJobRepository>();
            var service = new ExportService(mockRepo.Object);

            var userId = Guid.NewGuid();
            var jobId = Guid.NewGuid();

            var job = new UserExportJob
            {
                Id = jobId,
                UserId = userId,
                Status = ExportJobStatus.Running, // Ще не завершений!
                FileContent = new byte[] { 1, 2, 3 }
            };

            mockRepo
                .Setup(r => r.GetByIdWithContentAsync(userId, jobId))
                .ReturnsAsync(job);

            // 2. ACT
            var result = await service.GetFileAsync(userId, jobId);

            // 3. ASSERT
            Assert.Null(result);
        }

        [Fact]
        public async Task GetFileAsync_ShouldReturnNull_WhenFileContentIsNull()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IUserExportJobRepository>();
            var service = new ExportService(mockRepo.Object);

            var userId = Guid.NewGuid();
            var jobId = Guid.NewGuid();

            var job = new UserExportJob
            {
                Id = jobId,
                UserId = userId,
                Status = ExportJobStatus.Completed,
                FileContent = null
            };

            mockRepo
                .Setup(r => r.GetByIdWithContentAsync(userId, jobId))
                .ReturnsAsync(job);

            // 2. ACT
            var result = await service.GetFileAsync(userId, jobId);

            // 3. ASSERT
            Assert.Null(result);
        }

        [Fact]
        public async Task GetFileAsync_ShouldReturnFile_WhenJobIsCompleted()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IUserExportJobRepository>();
            var service = new ExportService(mockRepo.Object);

            var userId = Guid.NewGuid();
            var jobId = Guid.NewGuid();

            var fileBytes = new byte[] { 0x7B, 0x7D }; // "{}" in JSON
            var job = new UserExportJob
            {
                Id = jobId,
                UserId = userId,
                Status = ExportJobStatus.Completed,
                FileContent = fileBytes,
                FileName = "my-export.json",
                ContentType = "application/json"
            };

            mockRepo
                .Setup(r => r.GetByIdWithContentAsync(userId, jobId))
                .ReturnsAsync(job);

            // 2. ACT
            var result = await service.GetFileAsync(userId, jobId);

            // 3. ASSERT
            Assert.NotNull(result);
            var (content, fileName, contentType) = result.Value;
            Assert.Equal(fileBytes, content);
            Assert.Equal("my-export.json", fileName);
            Assert.Equal("application/json", contentType);
        }

        [Fact]
        public async Task GetFileAsync_ShouldUseDefaultFileName_WhenFileNameIsNull()
        {
            // 1. ARRANGE
            var mockRepo = new Mock<IUserExportJobRepository>();
            var service = new ExportService(mockRepo.Object);

            var userId = Guid.NewGuid();
            var jobId = Guid.NewGuid();

            var job = new UserExportJob
            {
                Id = jobId,
                UserId = userId,
                Status = ExportJobStatus.Completed,
                FileContent = new byte[] { 1 },
                FileName = null,
                ContentType = null
            };

            mockRepo
                .Setup(r => r.GetByIdWithContentAsync(userId, jobId))
                .ReturnsAsync(job);

            // 2. ACT
            var result = await service.GetFileAsync(userId, jobId);

            // 3. ASSERT
            Assert.NotNull(result);
            var (_, fileName, contentType) = result.Value;
            Assert.Equal($"export-{jobId}.json", fileName);
            Assert.Equal("application/json", contentType);
        }
    }
}
