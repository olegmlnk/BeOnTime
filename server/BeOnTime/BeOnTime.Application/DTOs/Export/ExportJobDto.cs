using BeOnTime.Core.Enums;

namespace BeOnTime.Application.DTOs.Export;

public class ExportJobDto
{
    public Guid Id { get; set; }
    public ExportJobStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public string? FileName { get; set; }
    public long? FileSizeBytes { get; set; }
}
