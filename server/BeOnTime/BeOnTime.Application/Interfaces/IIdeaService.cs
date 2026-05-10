using BeOnTime.Application.DTOs.Ideas;

namespace BeOnTime.Application.Interfaces;

public interface IIdeaService
{
    Task<IReadOnlyList<IdeaResponseDto>> GetAllAsync(Guid userId);
    Task<IdeaResponseDto?> GetByIdAsync(Guid userId, Guid id);
    Task<IdeaResponseDto> CreateAsync(Guid userId, CreateIdeaDto dto);
    Task<IdeaResponseDto?> UpdateAsync(Guid userId, Guid id, UpdateIdeaDto dto);
    Task<bool> DeleteAsync(Guid userId, Guid id);
    Task<ConvertIdeaResult?> ConvertToTaskAsync(Guid userId, Guid id);
}
