using BeOnTime.Application.Interfaces;
using BeOnTime.Core.Entities;
using BeOnTime.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace BeOnTime.Infrastructure.Repositories;

public class UserSettingsHistoryRepository : IUserSettingsHistoryRepository
{
    private readonly AppDbContext _context;

    public UserSettingsHistoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserSettingsHistory> CreateAsync(UserSettingsHistory snapshot)
    {
        await _context.UserSettingsHistories.AddAsync(snapshot);
        await _context.SaveChangesAsync();
        return snapshot;
    }

    public async Task<IReadOnlyList<UserSettingsHistory>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserSettingsHistories.AsNoTracking()
            .Where(h => h.UserId == userId)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();
    }

    public Task<UserSettingsHistory?> GetByIdAsync(Guid userId, Guid id) =>
        _context.UserSettingsHistories.AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);
}
