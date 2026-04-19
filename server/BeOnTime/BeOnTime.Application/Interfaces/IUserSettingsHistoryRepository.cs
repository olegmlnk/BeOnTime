using BeOnTime.Core.Entities;

namespace BeOnTime.Application.Interfaces;

public interface IUserSettingsHistoryRepository
{
    Task<UserSettingsHistory> CreateAsync(UserSettingsHistory snapshot);
    Task<IReadOnlyList<UserSettingsHistory>> GetByUserIdAsync(Guid userId);
    Task<UserSettingsHistory?> GetByIdAsync(Guid userId, Guid id);
}
