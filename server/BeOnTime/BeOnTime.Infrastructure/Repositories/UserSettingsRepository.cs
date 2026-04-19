using BeOnTime.Application.Interfaces;
using BeOnTime.Core.Entities;
using BeOnTime.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace BeOnTime.Infrastructure.Repositories;

public class UserSettingsRepository : IUserSettingsRepository
{
    private readonly AppDbContext _context;

    public UserSettingsRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<UserSettings?> GetByUserIdAsync(Guid userId) =>
        _context.UserSettings.FirstOrDefaultAsync(s => s.UserId == userId);

    public async Task<UserSettings> CreateAsync(UserSettings settings)
    {
        await _context.UserSettings.AddAsync(settings);
        await _context.SaveChangesAsync();
        return settings;
    }

    public async Task UpdateAsync(UserSettings settings)
    {
        _context.UserSettings.Update(settings);
        await _context.SaveChangesAsync();
    }
}
