using System.ComponentModel.DataAnnotations;

namespace BeOnTime.Application.DTOs.Reminders;

public class CreateReminderDto
{
    [Required]
    public Guid TaskId { get; set; }

    [Required]
    public DateTime RemindAt { get; set; }

    [MaxLength(500)]
    public string? Message { get; set; }
}
