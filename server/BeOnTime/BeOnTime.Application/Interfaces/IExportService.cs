using BeOnTime.Application.DTOs.Export;

namespace BeOnTime.Application.Interfaces;

public interface IExportService
{
    Task<ExportJobDto> EnqueueAsync(Guid userId);
    Task<ExportJobDto?> GetStatusAsync(Guid userId, Guid jobId);
    Task<(byte[] Content, string FileName, string ContentType)?> GetFileAsync(Guid userId, Guid jobId);
}
