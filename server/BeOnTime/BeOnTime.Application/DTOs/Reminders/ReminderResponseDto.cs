using BeOnTime.Core.Enums;

namespace BeOnTime.Application.DTOs.Reminders;

public class ReminderResponseDto
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public string TaskTitle { get; set; } = string.Empty;
    public DateTime RemindAt { get; set; }
    public ReminderStatus Status { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
