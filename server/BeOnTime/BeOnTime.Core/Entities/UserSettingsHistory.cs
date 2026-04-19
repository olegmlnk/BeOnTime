using BeOnTime.Core.Base;
using BeOnTime.Core.Enums;

namespace BeOnTime.Core.Entities;

public class UserSettingsHistory : BaseEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public UiTheme Theme { get; set; }
    public Language Language { get; set; }
    public string TimeZone { get; set; } = string.Empty;
    public DateFormat DateFormat { get; set; }
    public WeekStart WeekStart { get; set; }

    public TaskPriority DefaultTaskPriority { get; set; }
    public int DefaultReminderLeadMinutes { get; set; }
    public int UpcomingHorizonDays { get; set; }

    public bool RemindersSoundEnabled { get; set; }
    public int RemindersPollIntervalSeconds { get; set; }
    public bool ShowOverdueInBell { get; set; }
}
