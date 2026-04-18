using BeOnTime.Application.DTOs.Reminders;

namespace BeOnTime.Application.Interfaces;

public interface IReminderService
{
    Task<IReadOnlyList<ReminderResponseDto>> GetActiveAsync(Guid userId);
    Task<ReminderResult> CreateAsync(Guid userId, CreateReminderDto dto);
    Task<ReminderResult?> UpdateAsync(Guid userId, Guid id, UpdateReminderDto dto);
    Task<ReminderResponseDto?> DismissAsync(Guid userId, Guid id);
    Task<bool> DeleteAsync(Guid userId, Guid id);

    Task<int> ProcessDueRemindersAsync(CancellationToken cancellationToken);
}
