using Microsoft.EntityFrameworkCore;
using SpeededERP.Modules.Business.Comptabilite.Domain.Aggregates.Factures;
using SpeededERP.Modules.Business.Comptabilite.Domain.Repositories;
using SpeededERP.Modules.Business.Comptabilite.Infrastructure.Persistence;

namespace SpeededERP.Modules.Business.Comptabilite.Infrastructure.Persistence.Repositories;

public sealed class FactureRepository(ComptabiliteDbContext dbContext) : IFactureRepository
{
    public Task<Facture?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Factures.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

    public void Add(Facture aggregate) => dbContext.Factures.Add(aggregate);

    public void Update(Facture aggregate) => dbContext.Factures.Update(aggregate);

    public void Remove(Facture aggregate) => dbContext.Factures.Remove(aggregate);
}
