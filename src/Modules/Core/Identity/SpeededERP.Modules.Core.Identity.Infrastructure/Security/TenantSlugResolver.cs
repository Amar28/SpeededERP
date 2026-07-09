using SpeededERP.BuildingBlocks.Application.Security;
using SpeededERP.Modules.Core.Identity.Domain.Repositories;

namespace SpeededERP.Modules.Core.Identity.Infrastructure.Security;

public sealed class TenantSlugResolver(ITenantRepository tenantRepository) : ITenantSlugResolver
{
    public async Task<ResolvedTenant?> ResolveAsync(string slug, CancellationToken cancellationToken = default)
    {
        var tenant = await tenantRepository.GetBySlugAsync(slug, cancellationToken).ConfigureAwait(false);
        if (tenant is null)
        {
            return null;
        }

        var isolationMode = tenant.IsolationMode switch
        {
            Domain.Aggregates.Tenants.TenantIsolationMode.Shared => TenantIsolationMode.Shared,
            Domain.Aggregates.Tenants.TenantIsolationMode.Dedicated => TenantIsolationMode.Dedicated,
            Domain.Aggregates.Tenants.TenantIsolationMode.OnPremiseSingleTenant => TenantIsolationMode.OnPremiseSingleTenant,
            _ => TenantIsolationMode.Shared,
        };

        return new ResolvedTenant(tenant.Id, isolationMode);
    }
}
