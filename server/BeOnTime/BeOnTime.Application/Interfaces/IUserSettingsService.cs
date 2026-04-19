using BeOnTime.Application.DTOs.Settings;

namespace BeOnTime.Application.Interfaces;

public interface IUserSettingsService
{
    Task<UserSettingsDto> GetAsync(Guid userId);
    Task<UserSettingsDto> UpdateAsync(Guid userId, UpdateUserSettingsDto dto);
    Task<IReadOnlyList<UserSettingsHistoryDto>> GetHistoryAsync(Guid userId);
    Task<SettingsResult<UserSettingsDto>> RestoreFromHistoryAsync(Guid userId, Guid historyId);
}
