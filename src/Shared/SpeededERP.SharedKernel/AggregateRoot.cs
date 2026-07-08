namespace SpeededERP.SharedKernel;

/// <summary>
/// Non-generic marker so infrastructure (SaveChangesInterceptor, IDomainEventDispatcher)
/// can enumerate pending domain events across aggregates of different TId types without
/// reflection.
/// </summary>
public interface IHasDomainEvents
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    void ClearDomainEvents();
}

/// <summary>
/// An Entity that is the transactional consistency boundary of a cluster of objects
/// (DDD aggregate). Only aggregate roots are exposed through repositories.
/// </summary>
public abstract class AggregateRoot<TId> : Entity<TId>, IHasDomainEvents
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot()
    {
    }

    protected AggregateRoot(TId id)
        : base(id)
    {
    }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
