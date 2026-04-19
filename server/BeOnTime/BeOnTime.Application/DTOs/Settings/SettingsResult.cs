namespace BeOnTime.Application.DTOs.Settings;

public class SettingsResult<T> where T : class
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public T? Value { get; set; }

    public static SettingsResult<T> Ok(T value) =>
        new() { Success = true, Value = value };

    public static SettingsResult<T> Fail(string error) =>
        new() { Success = false, Error = error };
}
