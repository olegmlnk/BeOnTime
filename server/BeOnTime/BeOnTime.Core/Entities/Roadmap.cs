using BeOnTime.Core.Base;

namespace BeOnTime.Core.Entities;

public class Roadmap : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? TargetDate { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
