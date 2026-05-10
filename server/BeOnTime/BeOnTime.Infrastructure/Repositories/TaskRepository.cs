using BeOnTime.Application.DTOs.Tasks;
using BeOnTime.Application.Interfaces;
using BeOnTime.Core.Entities;
using BeOnTime.Core.Enums;
using BeOnTime.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace BeOnTime.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(Guid userId, TaskFilterDto filter)
    {
        var query = _context.Tasks.AsNoTracking().Where(t => t.UserId == userId);

        if (filter.Status.HasValue)
            query = query.Where(t => t.Status == filter.Status.Value);
        if (filter.Priority.HasValue)
            query = query.Where(t => t.Priority == filter.Priority.Value);
        if (filter.DeadlineFrom.HasValue)
            query = query.Where(t => t.Deadline >= filter.DeadlineFrom.Value);
        if (filter.DeadlineTo.HasValue)
            query = query.Where(t => t.Deadline <= filter.DeadlineTo.Value);
        if (filter.RoadmapId.HasValue)
            query = query.Where(t => t.RoadmapId == filter.RoadmapId.Value);
        else
            query = query.Where(t => t.RoadmapId == null); // Exclude roadmap steps from general task list

        return await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
    }

    public Task<TaskItem?> GetByIdAsync(Guid userId, Guid id) =>
        _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task UpdateAsync(TaskItem task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<TaskItem>> GetOverdueAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        return await _context.Tasks.AsNoTracking()
            .Where(t => t.UserId == userId
                        && t.RoadmapId == null // Exclude roadmap steps
                        && t.Deadline != null
                        && t.Deadline < now
                        && t.Status != TaskItemStatus.Done
                        && t.Status != TaskItemStatus.Cancelled)
            .OrderBy(t => t.Deadline)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<TaskItem>> GetUpcomingAsync(Guid userId, int days)
    {
        var now = DateTime.UtcNow;
        var upperBound = now.AddDays(days);
        return await _context.Tasks.AsNoTracking()
            .Where(t => t.UserId == userId
                        && t.RoadmapId == null // Exclude roadmap steps
                        && t.Deadline != null
                        && t.Deadline >= now
                        && t.Deadline <= upperBound
                        && t.Status != TaskItemStatus.Done
                        && t.Status != TaskItemStatus.Cancelled)
            .OrderBy(t => t.Deadline)
            .ToListAsync();
    }
}
