using BeOnTime.Application.DTOs.Archive;

namespace BeOnTime.Application.Interfaces;

public interface IArchiveService
{
    Task<IReadOnlyList<ArchiveItemDto>> GetAllAsync(Guid userId);
    Task<bool> RestoreAsync(Guid userId, string type, Guid id);
}
