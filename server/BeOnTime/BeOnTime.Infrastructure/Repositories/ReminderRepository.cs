using BeOnTime.Application.Interfaces;
using BeOnTime.Core.Entities;
using BeOnTime.Core.Enums;
using BeOnTime.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace BeOnTime.Infrastructure.Repositories;

public class ReminderRepository : IReminderRepository
{
    private readonly AppDbContext _context;

    public ReminderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Reminding>> GetActiveAsync(Guid userId)
    {
        return await _context.Remindings.AsNoTracking()
            .Include(r => r.Task)
            .Where(r => r.UserId == userId && r.Status != ReminderStatus.Dismissed)
            .OrderBy(r => r.RemindAt)
            .ToListAsync();
    }

    public Task<Reminding?> GetByIdAsync(Guid userId, Guid id) =>
        _context.Remindings
            .Include(r => r.Task)
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

    public async Task<Reminding> CreateAsync(Reminding reminder)
    {
        await _context.Remindings.AddAsync(reminder);
        await _context.SaveChangesAsync();
        return reminder;
    }

    public async Task UpdateAsync(Reminding reminder)
    {
        _context.Remindings.Update(reminder);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Reminding>> GetDueForProcessingAsync(DateTime now, int batchSize)
    {
        return await _context.Remindings
            .Include(r => r.Task)
            .Where(r => (r.Status == ReminderStatus.Pending || r.Status == ReminderStatus.Sent)
                        && r.RemindAt <= now)
            .OrderBy(r => r.RemindAt)
            .Take(batchSize)
            .ToListAsync();
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
