using Microsoft.EntityFrameworkCore;
using SpeededERP.Modules.Business.CRM.Domain.Repositories;
using SpeededERP.Modules.Business.CRM.Infrastructure.Persistence;
using DevisAggregate = SpeededERP.Modules.Business.CRM.Domain.Aggregates.Devis.Devis;

namespace SpeededERP.Modules.Business.CRM.Infrastructure.Persistence.Repositories;

public sealed class DevisRepository(CrmDbContext dbContext) : IDevisRepository
{
    // Owned collections (LigneDevis) are always loaded with their owner by EF Core —
    // no explicit .Include() needed.
    public Task<DevisAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.DevisSet.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public void Add(DevisAggregate aggregate) => dbContext.DevisSet.Add(aggregate);

    public void Update(DevisAggregate aggregate) => dbContext.DevisSet.Update(aggregate);

    public void Remove(DevisAggregate aggregate) => dbContext.DevisSet.Remove(aggregate);
}
