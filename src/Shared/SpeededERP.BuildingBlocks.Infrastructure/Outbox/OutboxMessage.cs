namespace SpeededERP.BuildingBlocks.Infrastructure.Outbox;

/// <summary>
/// Persisted, transactionally-consistent row backing the Outbox pattern (section 09,
/// Fig. 03). Written by <see cref="AppDbContextBase"/> in the same SaveChanges call as
/// the aggregate; read and published to MassTransit by <c>OutboxProcessor</c>.
/// </summary>
public sealed class OutboxMessage
{
    public Guid Id { get; init; }

    /// <summary>Assembly-qualified type name of the IntegrationEvent, for deserialization.</summary>
    public required string Type { get; init; }

    public required string Content { get; init; }

    public Guid TenantId { get; init; }

    public DateTimeOffset OccurredOnUtc { get; init; }

    public DateTimeOffset? ProcessedOnUtc { get; set; }

    public string? Error { get; set; }

    public int RetryCount { get; set; }
}
