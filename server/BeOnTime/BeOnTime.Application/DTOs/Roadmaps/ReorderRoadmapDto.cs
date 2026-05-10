using System.ComponentModel.DataAnnotations;

namespace BeOnTime.Application.DTOs.Roadmaps;

public class ReorderRoadmapDto
{
    [Required]
    public List<Guid> TaskIds { get; set; } = new();
}
