namespace BeOnTime.Application.DTOs.Roadmaps;

public class RoadmapListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? TargetDate { get; set; }
    public int TotalTasks { get; set; }
    public int DoneTasks { get; set; }
    public int ProgressPercent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
