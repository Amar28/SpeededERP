namespace SpeededERP.BuildingBlocks.Messaging;

/// <summary>
/// Base for every event published on the bus (MassTransit/RabbitMQ). Unlike domain
/// events, integration events cross module/process boundaries, so they must be stable,
/// versioned contracts — never a serialized aggregate (architecture section 09).
/// The producing module never references the consuming module: only this contract.
/// </summary>
public abstract record IntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();

    public DateTimeOffset OccurredOnUtc { get; init; } = DateTimeOffset.UtcNow;

    public Guid TenantId { get; init; }

    /// <summary>Correlates the event back to the request/saga that produced it.</summary>
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
