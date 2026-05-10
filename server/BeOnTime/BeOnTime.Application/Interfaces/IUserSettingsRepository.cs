using BeOnTime.Core.Entities;

namespace BeOnTime.Application.Interfaces;

public interface IUserSettingsRepository
{
    Task<UserSettings?> GetByUserIdAsync(Guid userId);
    Task<UserSettings> CreateAsync(UserSettings settings);
    Task UpdateAsync(UserSettings settings);
}
