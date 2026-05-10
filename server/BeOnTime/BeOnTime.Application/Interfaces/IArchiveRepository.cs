using BeOnTime.Application.DTOs.Archive;
using BeOnTime.Core.Entities;

namespace BeOnTime.Application.Interfaces;

public interface IArchiveRepository
{
    Task<IReadOnlyList<ArchiveItemDto>> GetAllAsync(Guid userId);
    Task<TaskItem?> GetDeletedTaskAsync(Guid userId, Guid id);
    Task<Idea?> GetDeletedIdeaAsync(Guid userId, Guid id);
    Task<Roadmap?> GetDeletedRoadmapAsync(Guid userId, Guid id);
    Task UpdateTaskAsync(TaskItem task);
    Task UpdateIdeaAsync(Idea idea);
    Task UpdateRoadmapAsync(Roadmap roadmap);
}
