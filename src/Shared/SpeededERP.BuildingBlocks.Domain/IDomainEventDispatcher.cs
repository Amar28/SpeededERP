using SpeededERP.SharedKernel;

namespace SpeededERP.BuildingBlocks.Domain;

/// <summary>
/// Dispatches the domain events raised by aggregates during a unit of work. The
/// interface stays here (Domain, zero external dependency); the MediatR-backed
/// implementation lives in BuildingBlocks.Application (see DomainEventNotification&lt;T&gt;).
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAndClearAsync(IEnumerable<IHasDomainEvents> aggregatesWithEvents, CancellationToken cancellationToken = default);
}
