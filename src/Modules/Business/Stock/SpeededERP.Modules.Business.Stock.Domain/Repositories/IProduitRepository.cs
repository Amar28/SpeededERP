using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Business.Stock.Domain.Aggregates.Produits;

namespace SpeededERP.Modules.Business.Stock.Domain.Repositories;

public interface IProduitRepository : IRepository<Produit, Guid>
{
    Task<bool> ReferenceExisteAsync(string reference, Guid tenantId, CancellationToken cancellationToken = default);
}
