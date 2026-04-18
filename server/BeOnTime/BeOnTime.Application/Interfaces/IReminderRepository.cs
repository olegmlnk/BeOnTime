using BeOnTime.Core.Entities;

namespace BeOnTime.Application.Interfaces;

public interface IReminderRepository
{
    Task<IReadOnlyList<Reminding>> GetActiveAsync(Guid userId);
    Task<Reminding?> GetByIdAsync(Guid userId, Guid id);
    Task<Reminding> CreateAsync(Reminding reminder);
    Task UpdateAsync(Reminding reminder);

    Task<IReadOnlyList<Reminding>> GetDueForProcessingAsync(DateTime now, int batchSize);
    Task SaveChangesAsync();
}
