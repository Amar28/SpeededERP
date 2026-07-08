using SpeededERP.BuildingBlocks.Messaging;

namespace SpeededERP.BuildingBlocks.Application.Outbox;

/// <summary>
/// Outbound port used by command handlers to enqueue an <see cref="IntegrationEvent"/>
/// in the same unit of work as the aggregate change. Implemented by each module's
/// DbContext (BuildingBlocks.Infrastructure.Persistence.AppDbContextBase) so the row is
/// committed atomically with the aggregate — see architecture section 09, Fig. 03.
/// </summary>
public interface IOutboxWriter
{
    Task EnqueueAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
}
