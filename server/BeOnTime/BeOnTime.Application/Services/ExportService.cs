using BeOnTime.Application.DTOs.Export;
using BeOnTime.Application.Interfaces;
using BeOnTime.Core.Entities;
using BeOnTime.Core.Enums;

namespace BeOnTime.Application.Services;

public class ExportService : IExportService
{
    private readonly IUserExportJobRepository _jobs;

    public ExportService(IUserExportJobRepository jobs)
    {
        _jobs = jobs;
    }

    public async Task<ExportJobDto> EnqueueAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        var job = new UserExportJob
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = ExportJobStatus.Pending,
            CreatedAt = now,
            UpdatedAt = now
        };
        await _jobs.CreateAsync(job);
        return Map(job);
    }

    public async Task<ExportJobDto?> GetStatusAsync(Guid userId, Guid jobId)
    {
        var job = await _jobs.GetByIdAsync(userId, jobId);
        return job is null ? null : Map(job);
    }

    public async Task<(byte[] Content, string FileName, string ContentType)?> GetFileAsync(Guid userId, Guid jobId)
    {
        var job = await _jobs.GetByIdWithContentAsync(userId, jobId);
        if (job is null || job.Status != ExportJobStatus.Completed || job.FileContent is null)
            return null;

        return (job.FileContent, job.FileName ?? $"export-{jobId}.json", job.ContentType ?? "application/json");
    }

    private static ExportJobDto Map(UserExportJob job) => new()
    {
        Id = job.Id,
        Status = job.Status,
        CreatedAt = job.CreatedAt,
        StartedAt = job.StartedAt,
        CompletedAt = job.CompletedAt,
        ErrorMessage = job.ErrorMessage,
        FileName = job.FileName,
        FileSizeBytes = job.FileSizeBytes
    };
}
