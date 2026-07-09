using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SpeededERP.BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// Applies pending EF Core migrations for <typeparamref name="TDbContext"/> once at
/// process startup — registered per module in SpeededERP.Worker, ahead of that
/// module's OutboxProcessor, so its outbox/business tables exist before anything
/// tries to read or write them (architecture section 06).
/// </summary>
public sealed class MigrationRunner<TDbContext>(
    IServiceScopeFactory scopeFactory,
    ILogger<MigrationRunner<TDbContext>> logger) : IHostedService
    where TDbContext : DbContext
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

        var pending = (await dbContext.Database.GetPendingMigrationsAsync(cancellationToken).ConfigureAwait(false)).ToList();
        if (pending.Count == 0)
        {
            return;
        }

        logger.LogInformation(
            "Applying {Count} pending migration(s) for {DbContext}: {Migrations}",
            pending.Count,
            typeof(TDbContext).Name,
            string.Join(", ", pending));

        await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
