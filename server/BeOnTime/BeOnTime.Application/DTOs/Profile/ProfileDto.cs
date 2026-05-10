using BeOnTime.Core.Enums;

namespace BeOnTime.Application.DTOs.Profile;

public class ProfileDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Role Role { get; set; }
    public DateTime CreatedAt { get; set; }
}
