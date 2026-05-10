using System.ComponentModel.DataAnnotations;

namespace BeOnTime.Application.DTOs.Ideas;

public class CreateIdeaDto
{
    [Required]
    [MinLength(1)]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    [MaxLength(5000)]
    public string Content { get; set; } = string.Empty;
}
