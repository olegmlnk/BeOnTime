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

    public async Task UpdateAsync(RefreshToken token)
    {
        _context.RefreshTokens.Update(token);
        await _context.SaveChangesAsync();
    }
}
