namespace BeOnTime.Application.DTOs.Reminders;

public class ReminderResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public ReminderResponseDto? Reminder { get; set; }

    public static ReminderResult Ok(ReminderResponseDto reminder) =>
        new() { Success = true, Reminder = reminder };

    public static ReminderResult Fail(string error) =>
        new() { Success = false, Error = error };
}
