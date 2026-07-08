using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Identity.Domain.Aggregates.Tenants;

namespace SpeededERP.Modules.Core.Identity.Domain.Repositories;

public interface ITenantRepository : IRepository<Tenant, Guid>
{
    Task<Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default);
}
