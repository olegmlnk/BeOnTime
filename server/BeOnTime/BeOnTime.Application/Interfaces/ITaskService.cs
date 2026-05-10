using BeOnTime.Application.DTOs.Tasks;
using BeOnTime.Core.Enums;

namespace BeOnTime.Application.Interfaces;

public interface ITaskService
{
    Task<IReadOnlyList<TaskResponseDto>> GetAllAsync(Guid userId, TaskFilterDto filter);
    Task<TaskResponseDto?> GetByIdAsync(Guid userId, Guid id);
    Task<TaskResponseDto> CreateAsync(Guid userId, CreateTaskDto dto);
    Task<TaskResponseDto?> UpdateAsync(Guid userId, Guid id, UpdateTaskDto dto);
    Task<TaskResponseDto?> UpdateStatusAsync(Guid userId, Guid id, TaskItemStatus status);
    Task<bool> DeleteAsync(Guid userId, Guid id);
    Task<IReadOnlyList<TaskResponseDto>> GetOverdueAsync(Guid userId);
    Task<IReadOnlyList<TaskResponseDto>> GetUpcomingAsync(Guid userId, int days);
}
