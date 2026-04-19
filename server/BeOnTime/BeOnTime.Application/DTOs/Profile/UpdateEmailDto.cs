namespace BeOnTime.Application.DTOs.Profile;

public class UpdateEmailDto
{
    public string Email { get; set; } = string.Empty;
    public string CurrentPassword { get; set; } = string.Empty;
}
