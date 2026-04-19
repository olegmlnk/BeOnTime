namespace BeOnTime.Application.DTOs.Profile;

public class ProfileResult<T> where T : class
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public T? Value { get; set; }

    public static ProfileResult<T> Ok(T value) =>
        new() { Success = true, Value = value };

    public static ProfileResult<T> Fail(string error) =>
        new() { Success = false, Error = error };
}
