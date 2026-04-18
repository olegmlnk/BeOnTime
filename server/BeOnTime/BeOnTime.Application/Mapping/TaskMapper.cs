using BeOnTime.Application.DTOs.Tasks;
using BeOnTime.Core.Entities;

namespace BeOnTime.Application.Mapping;

public static class TaskMapper
{
    public static TaskResponseDto Map(TaskItem task) => new()
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        Deadline = task.Deadline,
        Status = task.Status,
        Priority = task.Priority,
        RoadmapId = task.RoadmapId,
        OrderInRoadmap = task.OrderInRoadmap,
        CreatedAt = task.CreatedAt,
        UpdatedAt = task.UpdatedAt
    };
}
