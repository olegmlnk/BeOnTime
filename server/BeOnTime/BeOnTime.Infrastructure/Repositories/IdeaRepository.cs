using BeOnTime.Application.Interfaces;
using BeOnTime.Core.Entities;
using BeOnTime.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace BeOnTime.Infrastructure.Repositories;

public class IdeaRepository : IIdeaRepository
{
    private readonly AppDbContext _context;

    public IdeaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Idea>> GetAllAsync(Guid userId)
    {
        return await _context.Ideas.AsNoTracking()
            .Where(i => i.UserId == userId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public Task<Idea?> GetByIdAsync(Guid userId, Guid id) =>
        _context.Ideas.FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

    public async Task<Idea> CreateAsync(Idea idea)
    {
        await _context.Ideas.AddAsync(idea);
        await _context.SaveChangesAsync();
        return idea;
    }

    public async Task UpdateAsync(Idea idea)
    {
        _context.Ideas.Update(idea);
        await _context.SaveChangesAsync();
    }
}
