using BeOnTime.Application.DTOs.Roadmaps;

namespace BeOnTime.Application.Interfaces;

public interface IRoadmapService
{
    Task<IReadOnlyList<RoadmapListItemDto>> GetAllAsync(Guid userId);
    Task<RoadmapDetailsDto?> GetByIdAsync(Guid userId, Guid id);
    Task<RoadmapResult<RoadmapDetailsDto>> CreateAsync(Guid userId, CreateRoadmapDto dto);
    Task<RoadmapResult<RoadmapDetailsDto>?> UpdateAsync(Guid userId, Guid id, UpdateRoadmapDto dto);
    Task<bool> DeleteAsync(Guid userId, Guid id);
    Task<RoadmapResult<RoadmapDetailsDto>?> ReorderAsync(Guid userId, Guid id, ReorderRoadmapDto dto);
}
