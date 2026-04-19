namespace BeOnTime.Application.Interfaces;

public interface IExportJobRunner
{
    Task ProcessNextAsync(CancellationToken cancellationToken);
}
