using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SpeededERP.BuildingBlocks.Infrastructure.Outbox;

/// <summary>
/// Polls <c>{schema}.outbox_messages</c> for <typeparamref name="TDbContext"/> and
/// publishes unprocessed rows to MassTransit/RabbitMQ — the "Outbox Publisher" of
/// architecture Fig. 03. One instance is registered per module in SpeededERP.Worker.
/// </summary>
public sealed class OutboxProcessor<TDbContext>(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxProcessor<TDbContext>> logger)
    : BackgroundService
    where TDbContext : DbContext, IOutboxDbContext
{
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);
    private const int BatchSize = 20;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken).ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                logger.LogError(exception, "Échec du traitement de l'outbox pour {DbContext}", typeof(TDbContext).Name);
            }

            await Task.Delay(PollingInterval, stoppingToken).ConfigureAwait(false);
        }
    }

    private async Task ProcessBatchAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        var pending = await dbContext.OutboxMessages
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(BatchSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (var message in pending)
        {
            try
            {
                var eventType = Type.GetType(message.Type)
                    ?? throw new InvalidOperationException($"Type d'événement introuvable : {message.Type}");

                var integrationEvent = System.Text.Json.JsonSerializer.Deserialize(message.Content, eventType)
                    ?? throw new InvalidOperationException($"Désérialisation impossible pour {message.Type}");

                await publishEndpoint.Publish(integrationEvent, eventType, cancellationToken).ConfigureAwait(false);

                message.ProcessedOnUtc = DateTimeOffset.UtcNow;
                message.Error = null;
            }
            catch (Exception exception)
            {
                message.RetryCount++;
                message.Error = exception.Message;
                logger.LogWarning(
                    exception,
                    "Échec de publication du message outbox {MessageId} (tentative {RetryCount})",
                    message.Id,
                    message.RetryCount);
            }
        }

        if (pending.Count > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
