using BeOnTime.Application.DTOs.Ideas;
using BeOnTime.Application.DTOs.Tasks;
using BeOnTime.Application.Interfaces;
using BeOnTime.Core.Entities;
using BeOnTime.Core.Enums;

namespace BeOnTime.Application.Services;

public class IdeaService : IIdeaService
{
    private readonly IIdeaRepository _ideas;
    private readonly ITaskService _taskService;

    public IdeaService(IIdeaRepository ideas, ITaskService taskService)
    {
        _ideas = ideas;
        _taskService = taskService;
    }

    public async Task<IReadOnlyList<IdeaResponseDto>> GetAllAsync(Guid userId)
    {
        var ideas = await _ideas.GetAllAsync(userId);
        return ideas.Select(Map).ToList();
    }

    public async Task<IdeaResponseDto?> GetByIdAsync(Guid userId, Guid id)
    {
        var idea = await _ideas.GetByIdAsync(userId, id);
        return idea is null ? null : Map(idea);
    }

    public async Task<IdeaResponseDto> CreateAsync(Guid userId, CreateIdeaDto dto)
    {
        var now = DateTime.UtcNow;
        var idea = new Idea
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = dto.Title,
            Content = dto.Content,
            IsConvertedToTask = false,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _ideas.CreateAsync(idea);
        return Map(idea);
    }

    public async Task<IdeaResponseDto?> UpdateAsync(Guid userId, Guid id, UpdateIdeaDto dto)
    {
        var idea = await _ideas.GetByIdAsync(userId, id);
        if (idea is null) return null;

        idea.Title = dto.Title;
        idea.Content = dto.Content;
        idea.UpdatedAt = DateTime.UtcNow;

        await _ideas.UpdateAsync(idea);
        return Map(idea);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid id)
    {
        var idea = await _ideas.GetByIdAsync(userId, id);
        if (idea is null) return false;

        idea.DeletedAt = DateTime.UtcNow;
        idea.UpdatedAt = DateTime.UtcNow;

        await _ideas.UpdateAsync(idea);
        return true;
    }

    public async Task<ConvertIdeaResult?> ConvertToTaskAsync(Guid userId, Guid id)
    {
        var idea = await _ideas.GetByIdAsync(userId, id);
        if (idea is null) return null;

        if (idea.IsConvertedToTask)
            return ConvertIdeaResult.Fail("Idea has already been converted to a task");

        var task = await _taskService.CreateAsync(userId, new CreateTaskDto
        {
            Title = idea.Title,
            Description = idea.Content,
            Priority = TaskPriority.Medium
        });

        idea.IsConvertedToTask = true;
        idea.UpdatedAt = DateTime.UtcNow;
        await _ideas.UpdateAsync(idea);

        return ConvertIdeaResult.Ok(task);
    }

    private static IdeaResponseDto Map(Idea idea) => new()
    {
        Id = idea.Id,
        Title = idea.Title,
        Content = idea.Content,
        IsConvertedToTask = idea.IsConvertedToTask,
        CreatedAt = idea.CreatedAt,
        UpdatedAt = idea.UpdatedAt
    };
}
