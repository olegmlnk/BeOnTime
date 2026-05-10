using BeOnTime.Core.Base;
using BeOnTime.Core.Enums;

namespace BeOnTime.Core.Entities;

public class UserExportJob : BaseEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public ExportJobStatus Status { get; set; } = ExportJobStatus.Pending;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }

    public byte[]? FileContent { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public long? FileSizeBytes { get; set; }
}
