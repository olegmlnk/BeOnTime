using BeOnTime.Application.DTOs.Settings;
using BeOnTime.Core.Entities;

namespace BeOnTime.Application.Mapping;

public static class UserSettingsMapper
{
    public static UserSettingsDto Map(UserSettings entity) => new()
    {
        Id = entity.Id,
        Theme = entity.Theme,
        Language = entity.Language,
        TimeZone = entity.TimeZone,
        DateFormat = entity.DateFormat,
        WeekStart = entity.WeekStart,
        DefaultTaskPriority = entity.DefaultTaskPriority,
        DefaultReminderLeadMinutes = entity.DefaultReminderLeadMinutes,
        UpcomingHorizonDays = entity.UpcomingHorizonDays,
        RemindersSoundEnabled = entity.RemindersSoundEnabled,
        RemindersPollIntervalSeconds = entity.RemindersPollIntervalSeconds,
        ShowOverdueInBell = entity.ShowOverdueInBell,
        UpdatedAt = entity.UpdatedAt
    };

    public static UserSettingsHistoryDto MapHistory(UserSettingsHistory entity) => new()
    {
        Id = entity.Id,
        CreatedAt = entity.CreatedAt,
        Theme = entity.Theme,
        Language = entity.Language,
        TimeZone = entity.TimeZone,
        DateFormat = entity.DateFormat,
        WeekStart = entity.WeekStart,
        DefaultTaskPriority = entity.DefaultTaskPriority,
        DefaultReminderLeadMinutes = entity.DefaultReminderLeadMinutes,
        UpcomingHorizonDays = entity.UpcomingHorizonDays,
        RemindersSoundEnabled = entity.RemindersSoundEnabled,
        RemindersPollIntervalSeconds = entity.RemindersPollIntervalSeconds,
        ShowOverdueInBell = entity.ShowOverdueInBell
    };

    public static UserSettingsHistory Snapshot(UserSettings settings) => new()
    {
        Id = Guid.NewGuid(),
        UserId = settings.UserId,
        Theme = settings.Theme,
        Language = settings.Language,
        TimeZone = settings.TimeZone,
        DateFormat = settings.DateFormat,
        WeekStart = settings.WeekStart,
        DefaultTaskPriority = settings.DefaultTaskPriority,
        DefaultReminderLeadMinutes = settings.DefaultReminderLeadMinutes,
        UpcomingHorizonDays = settings.UpcomingHorizonDays,
        RemindersSoundEnabled = settings.RemindersSoundEnabled,
        RemindersPollIntervalSeconds = settings.RemindersPollIntervalSeconds,
        ShowOverdueInBell = settings.ShowOverdueInBell,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
}
