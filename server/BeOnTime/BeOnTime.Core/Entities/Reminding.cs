using BeOnTime.Core.Base;
using BeOnTime.Core.Enums;

namespace BeOnTime.Core.Entities;

public class Reminding : BaseEntity
{
    public Guid TaskId { get; set; }
    public TaskItem? Task { get; set; }

    public DateTime RemindAt { get; set; }
    public ReminderStatus Status { get; set; } = ReminderStatus.Pending;
    public string? Message { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }
}
