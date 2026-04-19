using BeOnTime.Core.Base;
using BeOnTime.Core.Enums;

namespace BeOnTime.Core.Entities;

public class UserSettings : BaseEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public UiTheme Theme { get; set; } = UiTheme.System;
    public Language Language { get; set; } = Language.Uk;
    public string TimeZone { get; set; } = "Europe/Kyiv";
    public DateFormat DateFormat { get; set; } = DateFormat.DayMonthYear;
    public WeekStart WeekStart { get; set; } = WeekStart.Monday;

    public TaskPriority DefaultTaskPriority { get; set; } = TaskPriority.Medium;
    public int DefaultReminderLeadMinutes { get; set; } = 15;
    public int UpcomingHorizonDays { get; set; } = 7;

    public bool RemindersSoundEnabled { get; set; } = true;
    public int RemindersPollIntervalSeconds { get; set; } = 60;
    public bool ShowOverdueInBell { get; set; } = true;
}
