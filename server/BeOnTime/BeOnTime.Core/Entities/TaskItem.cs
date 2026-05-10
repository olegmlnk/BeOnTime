using BeOnTime.Core.Base;
using BeOnTime.Core.Enums;

namespace BeOnTime.Core.Entities;

public class TaskItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public Guid? RoadmapId { get; set; }
    public int? OrderInRoadmap { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }
}
