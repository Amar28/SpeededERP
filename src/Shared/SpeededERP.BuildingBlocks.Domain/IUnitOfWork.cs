namespace SpeededERP.BuildingBlocks.Domain;

/// <summary>
/// One per module DbContext (one schema per module, section 06). Commits the
/// aggregate changes and, transactionally, the Outbox rows produced from the
/// aggregates' domain events (see BuildingBlocks.Messaging.OutboxMessage).
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
