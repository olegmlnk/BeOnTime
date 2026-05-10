using System.ComponentModel.DataAnnotations;
using BeOnTime.Core.Enums;

namespace BeOnTime.Application.DTOs.Tasks;

public class CreateTaskDto
{
    [Required]
    [MinLength(1)]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public DateTime? Deadline { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;

    public Guid? RoadmapId { get; set; }
}
