namespace BeOnTime.Application.DTOs.Roadmaps;

public class RoadmapResult<T> where T : class
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public T? Value { get; set; }

    public static RoadmapResult<T> Ok(T value) =>
        new() { Success = true, Value = value };

    public static RoadmapResult<T> Fail(string error) =>
        new() { Success = false, Error = error };
}
