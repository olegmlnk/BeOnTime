using System.ComponentModel.DataAnnotations;

namespace BeOnTime.Application.DTOs.Reminders;

public class UpdateReminderDto
{
    [Required]
    public DateTime RemindAt { get; set; }

    [MaxLength(500)]
    public string? Message { get; set; }
}
