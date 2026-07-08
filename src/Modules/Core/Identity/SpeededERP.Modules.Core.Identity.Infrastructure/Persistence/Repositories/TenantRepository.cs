using Microsoft.EntityFrameworkCore;
using SpeededERP.Modules.Core.Identity.Domain.Aggregates.Tenants;
using SpeededERP.Modules.Core.Identity.Domain.Repositories;

namespace SpeededERP.Modules.Core.Identity.Infrastructure.Persistence.Repositories;

public sealed class TenantRepository(IdentityModuleDbContext dbContext) : ITenantRepository
{
    public Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Tenants.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public Task<Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default) =>
        dbContext.Tenants.FirstOrDefaultAsync(t => t.Slug == slug, cancellationToken);

    public Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default) =>
        dbContext.Tenants.AnyAsync(t => t.Slug == slug, cancellationToken);

    public void Add(Tenant aggregate) => dbContext.Tenants.Add(aggregate);

    public void Update(Tenant aggregate) => dbContext.Tenants.Update(aggregate);

    public void Remove(Tenant aggregate) => dbContext.Tenants.Remove(aggregate);
}
