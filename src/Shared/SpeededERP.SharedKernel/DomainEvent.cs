namespace SpeededERP.SharedKernel;

/// <summary>
/// Marker for an event raised by an aggregate and dispatched in-memory, inside the
/// same transaction, before it is (optionally) translated into an IntegrationEvent
/// and written to the Outbox (see BuildingBlocks.Messaging and architecture section 09).
/// Deliberately free of any MediatR dependency: the Domain layer has zero external
/// dependencies (section 31). The MediatR adaptation happens in BuildingBlocks.Application
/// via DomainEventNotification&lt;T&gt;.
/// </summary>
public interface IDomainEvent
{
    Guid EventId { get; }

    DateTimeOffset OccurredOn { get; }
}

public abstract record DomainEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();

    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
