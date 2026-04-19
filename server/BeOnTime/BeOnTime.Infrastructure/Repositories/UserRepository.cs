using BeOnTime.Application.Interfaces;
using BeOnTime.Core.Entities;
using BeOnTime.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace BeOnTime.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByIdAsync(Guid id) =>
        _context.Users.FirstOrDefaultAsync(u => u.Id == id);

    public Task<User?> GetByEmailAsync(string email) =>
        _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public Task<User?> GetByUserNameAsync(string usernname) =>
        _context.Users.FirstOrDefaultAsync(u => u.UserName == usernname);

    public Task<bool> ExistsAsync(string email, string usernname) =>
        _context.Users.AnyAsync(u => u.Email == email || u.UserName == usernname);

    public Task<bool> ExistsExcludingAsync(string email, string usernname, Guid excludeUserId) =>
        _context.Users.AnyAsync(u => u.Id != excludeUserId
            && (u.Email == email || u.UserName == usernname));

    public async Task<User> CreateAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}
