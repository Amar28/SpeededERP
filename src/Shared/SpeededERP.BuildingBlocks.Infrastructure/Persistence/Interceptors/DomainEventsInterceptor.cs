using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.SharedKernel;

namespace SpeededERP.BuildingBlocks.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Dispatches every pending domain event, once the transaction that persisted the
/// originating aggregates has committed successfully — see
/// <see cref="Application.DomainEvents.MediatorDomainEventDispatcher"/>.
/// Deliberately post-commit: handlers here are for in-process, same-module,
/// non-transactional side effects only (logging, cache invalidation, scheduling a
/// background job). A significant event that must cross the module boundary is
/// <b>not</b> relayed through this dispatcher — the owning command handler enqueues its
/// IntegrationEvent via <c>IOutboxWriter</c> explicitly, in the same unit of work as the
/// aggregate change, before calling <c>SaveChangesAsync</c> (see
/// CRM's <c>AcceptDevisCommandHandler</c> for the reference example — architecture
/// section 09, Fig. 03). Doing the enqueue post-commit here instead would require a
/// second SaveChanges call and break the same-transaction guarantee.
/// </summary>
public sealed class DomainEventsInterceptor(IDomainEventDispatcher dispatcher) : SaveChangesInterceptor
{
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            var aggregatesWithEvents = eventData.Context.ChangeTracker
                .Entries<IHasDomainEvents>()
                .Select(entry => entry.Entity)
                .Where(entity => entity.DomainEvents.Count > 0)
                .ToList();

            if (aggregatesWithEvents.Count > 0)
            {
                await dispatcher.DispatchAndClearAsync(aggregatesWithEvents, cancellationToken).ConfigureAwait(false);
            }
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken).ConfigureAwait(false);
    }
}
