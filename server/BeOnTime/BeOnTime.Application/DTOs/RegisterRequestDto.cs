using System.ComponentModel.DataAnnotations;

namespace BeOnTime.Application.DTOs;

public class RegisterRequestDto
{
    [Required]
    [MinLength(3)]
    [MaxLength(55)]
    public string UserName { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
}