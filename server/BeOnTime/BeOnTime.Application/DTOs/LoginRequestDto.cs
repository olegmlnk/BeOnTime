using System.ComponentModel.DataAnnotations;

namespace BeOnTime.Application.DTOs;

public class LoginRequestDto
{
    public string? Username { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}