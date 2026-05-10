using System.ComponentModel.DataAnnotations;

namespace BeOnTime.Application.DTOs.Roadmaps;

public class UpdateRoadmapDto
{
    [Required]
    [MinLength(1)]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? TargetDate { get; set; }
}
