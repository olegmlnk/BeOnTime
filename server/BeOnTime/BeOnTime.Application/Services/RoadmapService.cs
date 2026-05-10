using BeOnTime.Application.DTOs.Roadmaps;
using BeOnTime.Application.Interfaces;
using BeOnTime.Application.Mapping;
using BeOnTime.Core.Entities;
using BeOnTime.Core.Enums;

namespace BeOnTime.Application.Services;

public class RoadmapService : IRoadmapService
{
    private readonly IRoadmapRepository _roadmaps;

    public RoadmapService(IRoadmapRepository roadmaps)
    {
        _roadmaps = roadmaps;
    }

    public Task<IReadOnlyList<RoadmapListItemDto>> GetAllAsync(Guid userId) =>
        _roadmaps.GetAllAsync(userId);

    public async Task<RoadmapDetailsDto?> GetByIdAsync(Guid userId, Guid id)
    {
        var roadmap = await _roadmaps.GetWithTasksAsync(userId, id);
        return roadmap is null ? null : MapDetails(roadmap);
    }

    public async Task<RoadmapResult<RoadmapDetailsDto>> CreateAsync(Guid userId, CreateRoadmapDto dto)
    {
        if (!AreDatesValid(dto.StartDate, dto.TargetDate))
            return RoadmapResult<RoadmapDetailsDto>.Fail("StartDate must be on or before TargetDate");

        var now = DateTime.UtcNow;
        var roadmap = new Roadmap
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = dto.Name,
            Description = dto.Description,
            StartDate = dto.StartDate,
            TargetDate = dto.TargetDate,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _roadmaps.CreateAsync(roadmap);
        return RoadmapResult<RoadmapDetailsDto>.Ok(MapDetails(roadmap));
    }

    public async Task<RoadmapResult<RoadmapDetailsDto>?> UpdateAsync(Guid userId, Guid id, UpdateRoadmapDto dto)
    {
        var roadmap = await _roadmaps.GetWithTasksAsync(userId, id);
        if (roadmap is null) return null;

        if (!AreDatesValid(dto.StartDate, dto.TargetDate))
            return RoadmapResult<RoadmapDetailsDto>.Fail("StartDate must be on or before TargetDate");

        roadmap.Name = dto.Name;
        roadmap.Description = dto.Description;
        roadmap.StartDate = dto.StartDate;
        roadmap.TargetDate = dto.TargetDate;
        roadmap.UpdatedAt = DateTime.UtcNow;

        await _roadmaps.UpdateAsync(roadmap);
        return RoadmapResult<RoadmapDetailsDto>.Ok(MapDetails(roadmap));
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid id)
    {
        var roadmap = await _roadmaps.GetByIdAsync(userId, id);
        if (roadmap is null) return false;

        var now = DateTime.UtcNow;
        var tasks = await _roadmaps.GetRoadmapTasksAsync(userId, id);
        foreach (var task in tasks)
        {
            task.RoadmapId = null;
            task.OrderInRoadmap = null;
            task.UpdatedAt = now;
        }

        roadmap.DeletedAt = now;
        roadmap.UpdatedAt = now;

        await _roadmaps.SaveChangesAsync();
        return true;
    }

    public async Task<RoadmapResult<RoadmapDetailsDto>?> ReorderAsync(Guid userId, Guid id, ReorderRoadmapDto dto)
    {
        var roadmap = await _roadmaps.GetByIdAsync(userId, id);
        if (roadmap is null) return null;

        var tasks = await _roadmaps.GetRoadmapTasksAsync(userId, id);
        var taskMap = tasks.ToDictionary(t => t.Id);

        if (dto.TaskIds.Count != tasks.Count
            || dto.TaskIds.Distinct().Count() != dto.TaskIds.Count
            || dto.TaskIds.Any(tid => !taskMap.ContainsKey(tid)))
        {
            return RoadmapResult<RoadmapDetailsDto>.Fail(
                "TaskIds must exactly match the set of tasks in this roadmap (no duplicates, no missing, no foreign).");
        }

        var now = DateTime.UtcNow;
        for (var i = 0; i < dto.TaskIds.Count; i++)
        {
            var task = taskMap[dto.TaskIds[i]];
            task.OrderInRoadmap = i;
            task.UpdatedAt = now;
        }

        roadmap.UpdatedAt = now;
        await _roadmaps.SaveChangesAsync();

        var refreshed = await _roadmaps.GetWithTasksAsync(userId, id);
        return RoadmapResult<RoadmapDetailsDto>.Ok(MapDetails(refreshed!));
    }

    private static bool AreDatesValid(DateTime? start, DateTime? target) =>
        !(start.HasValue && target.HasValue) || start.Value <= target.Value;

    private static RoadmapDetailsDto MapDetails(Roadmap roadmap)
    {
        var tasks = roadmap.Tasks ?? new List<TaskItem>();
        var total = tasks.Count;
        var done = tasks.Count(t => t.Status == TaskItemStatus.Done);

        return new RoadmapDetailsDto
        {
            Id = roadmap.Id,
            Name = roadmap.Name,
            Description = roadmap.Description,
            StartDate = roadmap.StartDate,
            TargetDate = roadmap.TargetDate,
            TotalTasks = total,
            DoneTasks = done,
            ProgressPercent = total == 0 ? 0 : (int)Math.Round(100.0 * done / total),
            CreatedAt = roadmap.CreatedAt,
            UpdatedAt = roadmap.UpdatedAt,
            Tasks = tasks.Select(TaskMapper.Map).ToList()
        };
    }
}
