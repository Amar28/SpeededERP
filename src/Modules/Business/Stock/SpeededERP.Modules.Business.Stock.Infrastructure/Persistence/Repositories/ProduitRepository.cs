using Microsoft.EntityFrameworkCore;
using SpeededERP.Modules.Business.Stock.Domain.Aggregates.Produits;
using SpeededERP.Modules.Business.Stock.Domain.Repositories;
using SpeededERP.Modules.Business.Stock.Infrastructure.Persistence;

namespace SpeededERP.Modules.Business.Stock.Infrastructure.Persistence.Repositories;

public sealed class ProduitRepository(StockDbContext dbContext) : IProduitRepository
{
    public Task<Produit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Produits.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<bool> ReferenceExisteAsync(string reference, Guid tenantId, CancellationToken cancellationToken = default) =>
        dbContext.Produits.AnyAsync(p => p.Reference == reference && p.TenantId == tenantId, cancellationToken);

    public void Add(Produit aggregate) => dbContext.Produits.Add(aggregate);

    public void Update(Produit aggregate) => dbContext.Produits.Update(aggregate);

    public void Remove(Produit aggregate) => dbContext.Produits.Remove(aggregate);
}
