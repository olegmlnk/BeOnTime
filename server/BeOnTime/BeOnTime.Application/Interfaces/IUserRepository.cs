using BeOnTime.Core.Entities;

namespace BeOnTime.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUserNameAsync(string usernname);
    Task<bool> ExistsAsync(string email, string usernname);
    Task<bool> ExistsExcludingAsync(string email, string usernname, Guid excludeUserId);
    Task<User> CreateAsync(User user);
    Task UpdateAsync(User user);
}
