using BeOnTime.Application.DTOs.Archive;
using BeOnTime.Application.Interfaces;
using BeOnTime.Core.Entities;
using BeOnTime.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace BeOnTime.Infrastructure.Repositories;

public class ArchiveRepository : IArchiveRepository
{
    private readonly AppDbContext _context;

    public ArchiveRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ArchiveItemDto>> GetAllAsync(Guid userId)
    {
        var tasks = await _context.Tasks.IgnoreQueryFilters().AsNoTracking()
            .Where(t => t.UserId == userId && t.DeletedAt != null)
            .Select(t => new ArchiveItemDto
            {
                Id = t.Id,
                Type = "task",
                Title = t.Title,
                DeletedAt = t.DeletedAt!.Value,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();

        var ideas = await _context.Ideas.IgnoreQueryFilters().AsNoTracking()
            .Where(i => i.UserId == userId && i.DeletedAt != null)
            .Select(i => new ArchiveItemDto
            {
                Id = i.Id,
                Type = "idea",
                Title = i.Title,
                DeletedAt = i.DeletedAt!.Value,
                CreatedAt = i.CreatedAt
            })
            .ToListAsync();

        var roadmaps = await _context.Roadmaps.IgnoreQueryFilters().AsNoTracking()
            .Where(r => r.UserId == userId && r.DeletedAt != null)
            .Select(r => new ArchiveItemDto
            {
                Id = r.Id,
                Type = "roadmap",
                Title = r.Name,
                DeletedAt = r.DeletedAt!.Value,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        return tasks.Concat(ideas).Concat(roadmaps)
            .OrderByDescending(x => x.DeletedAt)
            .ToList();
    }

    public Task<TaskItem?> GetDeletedTaskAsync(Guid userId, Guid id) =>
        _context.Tasks.IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId && t.DeletedAt != null);

    public Task<Idea?> GetDeletedIdeaAsync(Guid userId, Guid id) =>
        _context.Ideas.IgnoreQueryFilters()
            .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId && i.DeletedAt != null);

    public Task<Roadmap?> GetDeletedRoadmapAsync(Guid userId, Guid id) =>
        _context.Roadmaps.IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId && r.DeletedAt != null);

    public async Task UpdateTaskAsync(TaskItem task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateIdeaAsync(Idea idea)
    {
        _context.Ideas.Update(idea);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRoadmapAsync(Roadmap roadmap)
    {
        _context.Roadmaps.Update(roadmap);
        await _context.SaveChangesAsync();
    }
}
