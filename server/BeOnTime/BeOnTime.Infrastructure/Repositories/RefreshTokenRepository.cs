using BeOnTime.Application.Interfaces;
using BeOnTime.Core.Entities;
using BeOnTime.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace BeOnTime.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken> CreateAsync(RefreshToken token)
    {
        await _context.RefreshTokens.AddAsync(token);
        await _context.SaveChangesAsync();
        return token;
    }

    public Task<RefreshToken?> GetByTokenAsync(string token) =>
        _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);

    public Task<RefreshToken?> GetByIdAsync(Guid userId, Guid id) =>
        _context.RefreshTokens.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

    public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        return await _context.RefreshTokens.AsNoTracking()
            .Where(t => t.UserId == userId && !t.IsRevoked && t.Expires > now)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(RefreshToken token)
    {
        _context.RefreshTokens.Update(token);
        await _context.SaveChangesAsync();
    }

    public async Task<int> RevokeAllForUserAsync(Guid userId, string? exceptToken)
    {
        var now = DateTime.UtcNow;
        var query = _context.RefreshTokens
            .Where(t => t.UserId == userId && !t.IsRevoked);

        if (!string.IsNullOrEmpty(exceptToken))
            query = query.Where(t => t.Token != exceptToken);

        var tokens = await query.ToListAsync();
        foreach (var token in tokens)
        {
            token.IsRevoked = true;
            token.UpdatedAt = now;
        }

        if (tokens.Count > 0)
            await _context.SaveChangesAsync();

        return tokens.Count;
    }
}
