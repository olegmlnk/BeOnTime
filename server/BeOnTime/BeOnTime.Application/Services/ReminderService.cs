using BeOnTime.Application.DTOs.Reminders;
using BeOnTime.Application.Interfaces;
using BeOnTime.Core.Entities;
using BeOnTime.Core.Enums;

namespace BeOnTime.Application.Services;

public class ReminderService : IReminderService
{
    private const int ProcessingBatchSize = 500;

    private readonly IReminderRepository _reminders;
    private readonly ITaskRepository _tasks;

    public ReminderService(IReminderRepository reminders, ITaskRepository tasks)
    {
        _reminders = reminders;
        _tasks = tasks;
    }

    public async Task<IReadOnlyList<ReminderResponseDto>> GetActiveAsync(Guid userId)
    {
        var reminders = await _reminders.GetActiveAsync(userId);
        return reminders.Select(Map).ToList();
    }

    public async Task<ReminderResult> CreateAsync(Guid userId, CreateReminderDto dto)
    {
        if (dto.RemindAt <= DateTime.UtcNow)
            return ReminderResult.Fail("RemindAt must be in the future");

        var task = await _tasks.GetByIdAsync(userId, dto.TaskId);
        if (task is null)
            return ReminderResult.Fail("Task not found");

        var now = DateTime.UtcNow;
        var reminder = new Reminding
        {
            Id = Guid.NewGuid(),
            TaskId = task.Id,
            UserId = userId,
            RemindAt = dto.RemindAt,
            Status = ReminderStatus.Pending,
            Message = dto.Message,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _reminders.CreateAsync(reminder);
        reminder.Task = task;

        return ReminderResult.Ok(Map(reminder));
    }

    public async Task<ReminderResult?> UpdateAsync(Guid userId, Guid id, UpdateReminderDto dto)
    {
        var reminder = await _reminders.GetByIdAsync(userId, id);
        if (reminder is null) return null;

        if (reminder.Status == ReminderStatus.Dismissed)
            return ReminderResult.Fail("Dismissed reminder cannot be edited");

        if (dto.RemindAt <= DateTime.UtcNow)
            return ReminderResult.Fail("RemindAt must be in the future");

        reminder.RemindAt = dto.RemindAt;
        reminder.Message = dto.Message;
        reminder.Status = ReminderStatus.Pending;
        reminder.UpdatedAt = DateTime.UtcNow;

        await _reminders.UpdateAsync(reminder);
        return ReminderResult.Ok(Map(reminder));
    }

    public async Task<ReminderResponseDto?> DismissAsync(Guid userId, Guid id)
    {
        var reminder = await _reminders.GetByIdAsync(userId, id);
        if (reminder is null) return null;

        reminder.Status = ReminderStatus.Dismissed;
        reminder.UpdatedAt = DateTime.UtcNow;

        await _reminders.UpdateAsync(reminder);
        return Map(reminder);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid id)
    {
        var reminder = await _reminders.GetByIdAsync(userId, id);
        if (reminder is null) return false;

        reminder.DeletedAt = DateTime.UtcNow;
        reminder.UpdatedAt = DateTime.UtcNow;

        await _reminders.UpdateAsync(reminder);
        return true;
    }

    public async Task<int> ProcessDueRemindersAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var due = await _reminders.GetDueForProcessingAsync(now, ProcessingBatchSize);
        if (due.Count == 0) return 0;

        foreach (var reminder in due)
        {
            if (cancellationToken.IsCancellationRequested) break;

            var task = reminder.Task;
            var taskGone = task is null || task.DeletedAt != null
                || task.Status == TaskItemStatus.Done
                || task.Status == TaskItemStatus.Cancelled;

            if (taskGone)
            {
                reminder.Status = ReminderStatus.Dismissed;
            }
            else if (task!.Deadline.HasValue && task.Deadline.Value < now)
            {
                reminder.Status = ReminderStatus.Overdue;
            }
            else
            {
                reminder.Status = ReminderStatus.Sent;
            }

            reminder.UpdatedAt = now;
        }

        await _reminders.SaveChangesAsync();
        return due.Count;
    }

    private static ReminderResponseDto Map(Reminding reminder) => new()
    {
        Id = reminder.Id,
        TaskId = reminder.TaskId,
        TaskTitle = reminder.Task?.Title ?? string.Empty,
        RemindAt = reminder.RemindAt,
        Status = reminder.Status,
        Message = reminder.Message,
        CreatedAt = reminder.CreatedAt,
        UpdatedAt = reminder.UpdatedAt
    };
}
