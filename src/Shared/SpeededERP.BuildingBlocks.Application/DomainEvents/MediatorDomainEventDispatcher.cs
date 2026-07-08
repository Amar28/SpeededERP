using System.Reflection;
using MediatR;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.SharedKernel;

namespace SpeededERP.BuildingBlocks.Application.DomainEvents;

/// <summary>
/// Publishes every pending domain event of every changed aggregate through MediatR,
/// wrapped as <see cref="DomainEventNotification{TDomainEvent}"/>. Invoked by the
/// SaveChangesInterceptor (BuildingBlocks.Infrastructure) right after a successful commit.
/// </summary>
public sealed class MediatorDomainEventDispatcher(IPublisher publisher) : IDomainEventDispatcher
{
    private static readonly MethodInfo PublishDomainEventMethod =
        typeof(MediatorDomainEventDispatcher).GetMethod(nameof(PublishDomainEvent), BindingFlags.NonPublic | BindingFlags.Instance)!;

    public async Task DispatchAndClearAsync(IEnumerable<IHasDomainEvents> aggregatesWithEvents, CancellationToken cancellationToken = default)
    {
        var aggregates = aggregatesWithEvents.ToList();
        var domainEvents = aggregates.SelectMany(a => a.DomainEvents).ToList();

        foreach (var aggregate in aggregates)
        {
            aggregate.ClearDomainEvents();
        }

        foreach (var domainEvent in domainEvents)
        {
            var genericMethod = PublishDomainEventMethod.MakeGenericMethod(domainEvent.GetType());
            await ((Task)genericMethod.Invoke(this, [domainEvent, cancellationToken])!).ConfigureAwait(false);
        }
    }

    private Task PublishDomainEvent<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken)
        where TDomainEvent : IDomainEvent =>
        publisher.Publish(new DomainEventNotification<TDomainEvent>(domainEvent), cancellationToken);
}
