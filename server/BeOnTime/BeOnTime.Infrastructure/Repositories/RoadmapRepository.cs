using BeOnTime.Application.DTOs.Roadmaps;
using BeOnTime.Application.Interfaces;
using BeOnTime.Core.Entities;
using BeOnTime.Core.Enums;
using BeOnTime.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace BeOnTime.Infrastructure.Repositories;

public class RoadmapRepository : IRoadmapRepository
{
    private readonly AppDbContext _context;

    public RoadmapRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<RoadmapListItemDto>> GetAllAsync(Guid userId)
    {
        var items = await _context.Roadmaps.AsNoTracking()
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RoadmapListItemDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                StartDate = r.StartDate,
                TargetDate = r.TargetDate,
                TotalTasks = r.Tasks.Count,
                DoneTasks = r.Tasks.Count(t => t.Status == TaskItemStatus.Done),
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            })
            .ToListAsync();

        foreach (var item in items)
            item.ProgressPercent = item.TotalTasks == 0
                ? 0
                : (int)Math.Round(100.0 * item.DoneTasks / item.TotalTasks);

        return items;
    }

    public Task<Roadmap?> GetByIdAsync(Guid userId, Guid id) =>
        _context.Roadmaps.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

    public Task<Roadmap?> GetWithTasksAsync(Guid userId, Guid id) =>
        _context.Roadmaps
            .Include(r => r.Tasks.OrderBy(t => t.OrderInRoadmap ?? int.MaxValue)
                                 .ThenBy(t => t.CreatedAt))
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

    public async Task<Roadmap> CreateAsync(Roadmap roadmap)
    {
        await _context.Roadmaps.AddAsync(roadmap);
        await _context.SaveChangesAsync();
        return roadmap;
    }

    public async Task UpdateAsync(Roadmap roadmap)
    {
        _context.Roadmaps.Update(roadmap);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<TaskItem>> GetRoadmapTasksAsync(Guid userId, Guid roadmapId) =>
        await _context.Tasks
            .Where(t => t.RoadmapId == roadmapId && t.UserId == userId)
            .ToListAsync();

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
