using BeOnTime.Application.Interfaces;
using BeOnTime.Core.Entities;
using BeOnTime.Core.Enums;
using BeOnTime.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace BeOnTime.Infrastructure.Repositories;

public class UserExportJobRepository : IUserExportJobRepository
{
    private readonly AppDbContext _context;

    public UserExportJobRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserExportJob> CreateAsync(UserExportJob job)
    {
        await _context.UserExportJobs.AddAsync(job);
        await _context.SaveChangesAsync();
        return job;
    }

    public Task<UserExportJob?> GetByIdAsync(Guid userId, Guid id) =>
        _context.UserExportJobs.AsNoTracking()
            .Select(j => new UserExportJob
            {
                Id = j.Id,
                UserId = j.UserId,
                Status = j.Status,
                CreatedAt = j.CreatedAt,
                UpdatedAt = j.UpdatedAt,
                StartedAt = j.StartedAt,
                CompletedAt = j.CompletedAt,
                ErrorMessage = j.ErrorMessage,
                FileName = j.FileName,
                ContentType = j.ContentType,
                FileSizeBytes = j.FileSizeBytes
            })
            .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId);

    public Task<UserExportJob?> GetByIdWithContentAsync(Guid userId, Guid id) =>
        _context.UserExportJobs.AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId);

    public Task<UserExportJob?> GetNextPendingAsync() =>
        _context.UserExportJobs
            .Where(j => j.Status == ExportJobStatus.Pending)
            .OrderBy(j => j.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task UpdateAsync(UserExportJob job)
    {
        _context.UserExportJobs.Update(job);
        await _context.SaveChangesAsync();
    }
}
