using BeOnTime.Core.Base;

namespace BeOnTime.Core.Entities;

public class Idea : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsConvertedToTask { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }
}
