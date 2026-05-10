namespace BeOnTime.Application.DTOs.Sessions;

public class SessionDto
{
    public Guid Id { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; }
    public bool IsCurrent { get; set; }
}
