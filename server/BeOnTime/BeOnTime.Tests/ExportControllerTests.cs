using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;
using BeOnTime.API.Controllers;
using BeOnTime.Application.Interfaces;
using BeOnTime.Application.DTOs.Export;

namespace BeOnTime.Tests
{
    public class ExportControllerTests
    {
        private ExportController CreateControllerWithUser(Mock<IExportService> mockService, Guid userId)
        {
            var controller = new ExportController(mockService.Object);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            return controller;
        }

        [Fact]
        public async Task Enqueue_ShouldReturnAccepted_WithJobDto()
        {
            var mockService = new Mock<IExportService>();
            var userId = Guid.NewGuid();
            var jobId = Guid.NewGuid();

            var job = new ExportJobDto { Id = jobId, CreatedAt = DateTime.UtcNow };

            mockService
                .Setup(s => s.EnqueueAsync(userId))
                .ReturnsAsync(job);

            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Enqueue();

            var acceptedResult = Assert.IsType<AcceptedAtActionResult>(result);
            var returnedJob = Assert.IsType<ExportJobDto>(acceptedResult.Value);
            Assert.Equal(jobId, returnedJob.Id);
        }

        [Fact]
        public async Task GetStatus_ShouldReturnOk_WhenJobExists()
        {
            var mockService = new Mock<IExportService>();
            var userId = Guid.NewGuid();
            var jobId = Guid.NewGuid();

            var job = new ExportJobDto { Id = jobId };

            mockService
                .Setup(s => s.GetStatusAsync(userId, jobId))
                .ReturnsAsync(job);

            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.GetStatus(jobId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedJob = Assert.IsType<ExportJobDto>(okResult.Value);
            Assert.Equal(jobId, returnedJob.Id);
        }

        [Fact]
        public async Task GetStatus_ShouldReturnNotFound_WhenJobDoesNotExist()
        {
            var mockService = new Mock<IExportService>();
            var userId = Guid.NewGuid();
            var jobId = Guid.NewGuid();

            mockService
                .Setup(s => s.GetStatusAsync(userId, jobId))
                .ReturnsAsync((ExportJobDto?)null);

            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.GetStatus(jobId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Download_ShouldReturnFile_WhenFileExists()
        {
            var mockService = new Mock<IExportService>();
            var userId = Guid.NewGuid();
            var jobId = Guid.NewGuid();

            var fileContent = new byte[] { 1, 2, 3, 4, 5 };
            var fileName = "export.zip";
            var contentType = "application/zip";

            mockService
                .Setup(s => s.GetFileAsync(userId, jobId))
                .ReturnsAsync((fileContent, fileName, contentType));

            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Download(jobId);

            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal(fileContent, fileResult.FileContents);
            Assert.Equal(fileName, fileResult.FileDownloadName);
            Assert.Equal(contentType, fileResult.ContentType);
        }

        [Fact]
        public async Task Download_ShouldReturnNotFound_WhenFileDoesNotExist()
        {
            var mockService = new Mock<IExportService>();
            var userId = Guid.NewGuid();
            var jobId = Guid.NewGuid();

            mockService
                .Setup(s => s.GetFileAsync(userId, jobId))
                .ReturnsAsync(((byte[], string, string)?)null);

            var controller = CreateControllerWithUser(mockService, userId);

            var result = await controller.Download(jobId);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
