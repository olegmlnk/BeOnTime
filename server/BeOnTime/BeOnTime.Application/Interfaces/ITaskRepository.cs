using BeOnTime.Application.DTOs.Tasks;
using BeOnTime.Core.Entities;

namespace BeOnTime.Application.Interfaces;

public interface ITaskRepository
{
    Task<IReadOnlyList<TaskItem>> GetAllAsync(Guid userId, TaskFilterDto filter);
    Task<TaskItem?> GetByIdAsync(Guid userId, Guid id);
    Task<TaskItem> CreateAsync(TaskItem task);
    Task UpdateAsync(TaskItem task);
    Task<IReadOnlyList<TaskItem>> GetOverdueAsync(Guid userId);
    Task<IReadOnlyList<TaskItem>> GetUpcomingAsync(Guid userId, int days);
}
