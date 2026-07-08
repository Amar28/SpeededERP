using SpeededERP.SharedKernel;

namespace SpeededERP.BuildingBlocks.Domain;

/// <summary>
/// Outbound port for aggregate persistence — implemented once per aggregate in each
/// module's Infrastructure layer (e.g. CRM.Infrastructure.Persistence.DevisRepository).
/// Deliberately minimal: no IQueryable leakage, no generic Find-by-spec. Reads for
/// screens go through dedicated read models (section 08), not through this port.
/// </summary>
public interface IRepository<TAggregate, in TId>
    where TAggregate : AggregateRoot<TId>
    where TId : notnull
{
    Task<TAggregate?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    void Add(TAggregate aggregate);

    void Update(TAggregate aggregate);

    void Remove(TAggregate aggregate);
}
