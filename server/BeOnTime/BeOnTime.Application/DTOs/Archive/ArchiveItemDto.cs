namespace BeOnTime.Application.DTOs.Archive;

public class ArchiveItemDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
