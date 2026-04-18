using BeOnTime.Application.DTOs.Tasks;

namespace BeOnTime.Application.DTOs.Ideas;

public class ConvertIdeaResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public TaskResponseDto? Task { get; set; }

    public static ConvertIdeaResult Ok(TaskResponseDto task) =>
        new() { Success = true, Task = task };

    public static ConvertIdeaResult Fail(string error) =>
        new() { Success = false, Error = error };
}
