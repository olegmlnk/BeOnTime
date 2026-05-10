using BeOnTime.Application.DTOs.Archive;
using BeOnTime.Application.Interfaces;

namespace BeOnTime.Application.Services;

public class ArchiveService : IArchiveService
{
    private readonly IArchiveRepository _archive;

    public ArchiveService(IArchiveRepository archive)
    {
        _archive = archive;
    }

    public Task<IReadOnlyList<ArchiveItemDto>> GetAllAsync(Guid userId) =>
        _archive.GetAllAsync(userId);

    public async Task<bool> RestoreAsync(Guid userId, string type, Guid id)
    {
        var now = DateTime.UtcNow;

        switch (type?.ToLowerInvariant())
        {
            case "task":
                var task = await _archive.GetDeletedTaskAsync(userId, id);
                if (task is null) return false;
                task.DeletedAt = null;
                task.UpdatedAt = now;
                await _archive.UpdateTaskAsync(task);
                return true;

            case "idea":
                var idea = await _archive.GetDeletedIdeaAsync(userId, id);
                if (idea is null) return false;
                idea.DeletedAt = null;
                idea.UpdatedAt = now;
                await _archive.UpdateIdeaAsync(idea);
                return true;

            case "roadmap":
                var roadmap = await _archive.GetDeletedRoadmapAsync(userId, id);
                if (roadmap is null) return false;
                roadmap.DeletedAt = null;
                roadmap.UpdatedAt = now;
                await _archive.UpdateRoadmapAsync(roadmap);
                return true;

            default:
                return false;
        }
    }
}
