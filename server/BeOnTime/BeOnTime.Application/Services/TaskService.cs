using BeOnTime.Application.DTOs.Tasks;
using BeOnTime.Application.Interfaces;
using BeOnTime.Application.Mapping;
using BeOnTime.Core.Entities;
using BeOnTime.Core.Enums;

namespace BeOnTime.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _tasks;

    public TaskService(ITaskRepository tasks)
    {
        _tasks = tasks;
    }

    public async Task<IReadOnlyList<TaskResponseDto>> GetAllAsync(Guid userId, TaskFilterDto filter)
    {
        var tasks = await _tasks.GetAllAsync(userId, filter);
        return tasks.Select(TaskMapper.Map).ToList();
    }

    public async Task<TaskResponseDto?> GetByIdAsync(Guid userId, Guid id)
    {
        var task = await _tasks.GetByIdAsync(userId, id);
        return task is null ? null : TaskMapper.Map(task);
    }

    public async Task<TaskResponseDto> CreateAsync(Guid userId, CreateTaskDto dto)
    {
        var now = DateTime.UtcNow;
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = dto.Title,
            Description = dto.Description,
            Deadline = dto.Deadline,
            Priority = dto.Priority,
            Status = TaskItemStatus.Todo,
            RoadmapId = dto.RoadmapId,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _tasks.CreateAsync(task);
        return TaskMapper.Map(task);
    }

    public async Task<TaskResponseDto?> UpdateAsync(Guid userId, Guid id, UpdateTaskDto dto)
    {
        var task = await _tasks.GetByIdAsync(userId, id);
        if (task is null) return null;

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Deadline = dto.Deadline;
        task.Status = dto.Status;
        task.Priority = dto.Priority;
        task.RoadmapId = dto.RoadmapId;
        task.UpdatedAt = DateTime.UtcNow;

        await _tasks.UpdateAsync(task);
        return TaskMapper.Map(task);
    }

    public async Task<TaskResponseDto?> UpdateStatusAsync(Guid userId, Guid id, TaskItemStatus status)
    {
        var task = await _tasks.GetByIdAsync(userId, id);
        if (task is null) return null;

        task.Status = status;
        task.UpdatedAt = DateTime.UtcNow;

        await _tasks.UpdateAsync(task);
        return TaskMapper.Map(task);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid id)
    {
        var task = await _tasks.GetByIdAsync(userId, id);
        if (task is null) return false;

        task.DeletedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        await _tasks.UpdateAsync(task);
        return true;
    }

    public async Task<IReadOnlyList<TaskResponseDto>> GetOverdueAsync(Guid userId)
    {
        var tasks = await _tasks.GetOverdueAsync(userId);
        return tasks.Select(TaskMapper.Map).ToList();
    }

    public async Task<IReadOnlyList<TaskResponseDto>> GetUpcomingAsync(Guid userId, int days)
    {
        if (days < 1) days = 7;
        var tasks = await _tasks.GetUpcomingAsync(userId, days);
        return tasks.Select(TaskMapper.Map).ToList();
    }

}
