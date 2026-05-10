using System.Text;
using System.Text.Json;
using BeOnTime.Application.Interfaces;
using BeOnTime.Core.Entities;
using BeOnTime.Core.Enums;
using BeOnTime.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BeOnTime.Infrastructure.BackgroundServices;

public class ExportJobRunner : IExportJobRunner
{
    private readonly AppDbContext _context;
    private readonly ILogger<ExportJobRunner> _logger;

    public ExportJobRunner(AppDbContext context, ILogger<ExportJobRunner> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ProcessNextAsync(CancellationToken cancellationToken)
    {
        var job = await _context.UserExportJobs
            .Where(j => j.Status == ExportJobStatus.Pending)
            .OrderBy(j => j.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (job is null) return;

        var now = DateTime.UtcNow;
        job.Status = ExportJobStatus.Running;
        job.StartedAt = now;
        job.UpdatedAt = now;
        await _context.SaveChangesAsync(cancellationToken);

        try
        {
            var payload = await BuildPayloadAsync(job.UserId, cancellationToken);
            var bytes = JsonSerializer.SerializeToUtf8Bytes(payload, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            var completedAt = DateTime.UtcNow;
            job.FileContent = bytes;
            job.FileName = $"beontime-export-{job.Id}.json";
            job.ContentType = "application/json";
            job.FileSizeBytes = bytes.LongLength;
            job.Status = ExportJobStatus.Completed;
            job.CompletedAt = completedAt;
            job.UpdatedAt = completedAt;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Export job {JobId} completed ({Size} bytes)", job.Id, bytes.LongLength);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Export job {JobId} failed", job.Id);
            job.Status = ExportJobStatus.Failed;
            job.ErrorMessage = ex.Message.Length > 2000 ? ex.Message[..2000] : ex.Message;
            job.CompletedAt = DateTime.UtcNow;
            job.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(CancellationToken.None);
        }
    }

    private async Task<object> BuildPayloadAsync(Guid userId, CancellationToken ct)
    {
        var user = await _context.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        var tasks = await _context.Tasks.IgnoreQueryFilters().AsNoTracking()
            .Where(t => t.UserId == userId)
            .ToListAsync(ct);

        var ideas = await _context.Ideas.IgnoreQueryFilters().AsNoTracking()
            .Where(i => i.UserId == userId)
            .ToListAsync(ct);

        var roadmaps = await _context.Roadmaps.IgnoreQueryFilters().AsNoTracking()
            .Where(r => r.UserId == userId)
            .ToListAsync(ct);

        var settings = await _context.UserSettings.AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId, ct);

        var settingsHistory = await _context.UserSettingsHistories.AsNoTracking()
            .Where(h => h.UserId == userId)
            .OrderBy(h => h.CreatedAt)
            .ToListAsync(ct);

        return new
        {
            ExportedAt = DateTime.UtcNow,
            User = user is null ? null : new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.Role,
                user.CreatedAt
            },
            Settings = settings,
            SettingsHistory = settingsHistory,
            Tasks = tasks,
            Ideas = ideas,
            Roadmaps = roadmaps
        };
    }
}
