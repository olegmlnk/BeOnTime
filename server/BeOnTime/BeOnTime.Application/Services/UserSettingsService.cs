using BeOnTime.Application.DTOs.Settings;
using BeOnTime.Application.Interfaces;
using BeOnTime.Application.Mapping;
using BeOnTime.Core.Entities;

namespace BeOnTime.Application.Services;

public class UserSettingsService : IUserSettingsService
{
    private readonly IUserSettingsRepository _settings;
    private readonly IUserSettingsHistoryRepository _history;

    public UserSettingsService(
        IUserSettingsRepository settings,
        IUserSettingsHistoryRepository history)
    {
        _settings = settings;
        _history = history;
    }

    public async Task<UserSettingsDto> GetAsync(Guid userId)
    {
        var settings = await _settings.GetByUserIdAsync(userId) ?? await CreateDefaultsAsync(userId);
        return UserSettingsMapper.Map(settings);
    }

    public async Task<UserSettingsDto> UpdateAsync(Guid userId, UpdateUserSettingsDto dto)
    {
        var settings = await _settings.GetByUserIdAsync(userId) ?? await CreateDefaultsAsync(userId);

        await _history.CreateAsync(UserSettingsMapper.Snapshot(settings));

        Apply(settings, dto);
        settings.UpdatedAt = DateTime.UtcNow;
        await _settings.UpdateAsync(settings);

        return UserSettingsMapper.Map(settings);
    }

    public async Task<IReadOnlyList<UserSettingsHistoryDto>> GetHistoryAsync(Guid userId)
    {
        var items = await _history.GetByUserIdAsync(userId);
        return items.Select(UserSettingsMapper.MapHistory).ToList();
    }

    public async Task<SettingsResult<UserSettingsDto>> RestoreFromHistoryAsync(Guid userId, Guid historyId)
    {
        var snapshot = await _history.GetByIdAsync(userId, historyId);
        if (snapshot is null)
            return SettingsResult<UserSettingsDto>.Fail("History entry not found");

        var settings = await _settings.GetByUserIdAsync(userId) ?? await CreateDefaultsAsync(userId);

        await _history.CreateAsync(UserSettingsMapper.Snapshot(settings));

        settings.Theme = snapshot.Theme;
        settings.Language = snapshot.Language;
        settings.TimeZone = snapshot.TimeZone;
        settings.DateFormat = snapshot.DateFormat;
        settings.WeekStart = snapshot.WeekStart;
        settings.DefaultTaskPriority = snapshot.DefaultTaskPriority;
        settings.DefaultReminderLeadMinutes = snapshot.DefaultReminderLeadMinutes;
        settings.UpcomingHorizonDays = snapshot.UpcomingHorizonDays;
        settings.RemindersSoundEnabled = snapshot.RemindersSoundEnabled;
        settings.RemindersPollIntervalSeconds = snapshot.RemindersPollIntervalSeconds;
        settings.ShowOverdueInBell = snapshot.ShowOverdueInBell;
        settings.UpdatedAt = DateTime.UtcNow;

        await _settings.UpdateAsync(settings);

        return SettingsResult<UserSettingsDto>.Ok(UserSettingsMapper.Map(settings));
    }

    private async Task<UserSettings> CreateDefaultsAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        var settings = new UserSettings
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreatedAt = now,
            UpdatedAt = now
        };
        return await _settings.CreateAsync(settings);
    }

    private static void Apply(UserSettings entity, UpdateUserSettingsDto dto)
    {
        entity.Theme = dto.Theme;
        entity.Language = dto.Language;
        entity.TimeZone = string.IsNullOrWhiteSpace(dto.TimeZone) ? entity.TimeZone : dto.TimeZone;
        entity.DateFormat = dto.DateFormat;
        entity.WeekStart = dto.WeekStart;
        entity.DefaultTaskPriority = dto.DefaultTaskPriority;
        entity.DefaultReminderLeadMinutes = dto.DefaultReminderLeadMinutes < 0 ? 0 : dto.DefaultReminderLeadMinutes;
        entity.UpcomingHorizonDays = dto.UpcomingHorizonDays < 1 ? 7 : dto.UpcomingHorizonDays;
        entity.RemindersSoundEnabled = dto.RemindersSoundEnabled;
        entity.RemindersPollIntervalSeconds = dto.RemindersPollIntervalSeconds < 15 ? 60 : dto.RemindersPollIntervalSeconds;
        entity.ShowOverdueInBell = dto.ShowOverdueInBell;
    }
}
