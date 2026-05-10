using BeOnTime.Core.Entities;

namespace BeOnTime.Application.Interfaces;

public interface IUserExportJobRepository
{
    Task<UserExportJob> CreateAsync(UserExportJob job);
    Task<UserExportJob?> GetByIdAsync(Guid userId, Guid id);
    Task<UserExportJob?> GetByIdWithContentAsync(Guid userId, Guid id);
    Task<UserExportJob?> GetNextPendingAsync();
    Task UpdateAsync(UserExportJob job);
}
