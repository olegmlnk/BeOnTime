using BeOnTime.Application.DTOs.Roadmaps;
using BeOnTime.Core.Entities;

namespace BeOnTime.Application.Interfaces;

public interface IRoadmapRepository
{
    Task<IReadOnlyList<RoadmapListItemDto>> GetAllAsync(Guid userId);
    Task<Roadmap?> GetByIdAsync(Guid userId, Guid id);
    Task<Roadmap?> GetWithTasksAsync(Guid userId, Guid id);
    Task<Roadmap> CreateAsync(Roadmap roadmap);
    Task UpdateAsync(Roadmap roadmap);
    Task<IReadOnlyList<TaskItem>> GetRoadmapTasksAsync(Guid userId, Guid roadmapId);
    Task SaveChangesAsync();
}
