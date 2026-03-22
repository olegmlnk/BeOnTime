using BeOnTime.Application.Interfaces;
using BeOnTime.Core.Entities;
using BeOnTime.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace BeOnTime.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.FindAsync<User>(id);    
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByUserNameAsync(string usernname)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserName == usernname);
    }

    public async Task<bool> ExistsAsync(string email, string usernname)
    {
        return await _context.Users.AnyAsync(u => u.Email == email && u.UserName == usernname);
    }

    public async Task<User> CreateAsync(User user)
    {
         await _context.Users.AddAsync(user);
         await _context.SaveChangesAsync();
         return user;
    }
}