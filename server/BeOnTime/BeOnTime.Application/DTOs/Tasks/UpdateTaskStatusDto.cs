using System.ComponentModel.DataAnnotations;
using BeOnTime.Core.Enums;

namespace BeOnTime.Application.DTOs.Tasks;

public class UpdateTaskStatusDto
{
    [Required]
    public TaskItemStatus Status { get; set; }
}
