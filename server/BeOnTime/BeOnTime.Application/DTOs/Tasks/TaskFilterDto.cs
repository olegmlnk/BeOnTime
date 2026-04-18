using BeOnTime.Core.Enums;

namespace BeOnTime.Application.DTOs.Tasks;

public class TaskFilterDto
{
    public TaskItemStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }
    public DateTime? DeadlineFrom { get; set; }
    public DateTime? DeadlineTo { get; set; }
    public Guid? RoadmapId { get; set; }
}
