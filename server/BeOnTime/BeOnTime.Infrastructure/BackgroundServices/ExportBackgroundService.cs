using BeOnTime.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BeOnTime.Infrastructure.BackgroundServices;

public class ExportBackgroundService : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(10);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ExportBackgroundService> _logger;

    public ExportBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<ExportBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ExportBackgroundService started");
        using var timer = new PeriodicTimer(PollInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DrainPendingAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExportBackgroundService loop error");
            }

            try
            {
                await timer.WaitForNextTickAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("ExportBackgroundService stopped");
    }

    private async Task DrainPendingAsync(CancellationToken ct)
    {
        for (var i = 0; i < 10 && !ct.IsCancellationRequested; i++)
        {
            using var scope = _scopeFactory.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IExportJobRunner>();
            await runner.ProcessNextAsync(ct);
        }
    }
}
