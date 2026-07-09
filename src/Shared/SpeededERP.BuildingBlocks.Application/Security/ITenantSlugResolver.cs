namespace SpeededERP.BuildingBlocks.Application.Security;

/// <summary>
/// Resolves a tenant's identity from its public slug — used by
/// <c>TenantResolutionMiddleware</c> (BuildingBlocks.Web) for pre-authentication
/// endpoints that need a tenant (e.g. registering a brand-new organization's first
/// user) but have no JWT <c>tenant_id</c> claim yet to read from
/// (<see cref="ITenantContext"/>'s usual source — see architecture section 17).
/// Implemented in Core.Identity.Infrastructure, the only module that owns tenant data.
/// </summary>
public interface ITenantSlugResolver
{
    Task<ResolvedTenant?> ResolveAsync(string slug, CancellationToken cancellationToken = default);
}

public sealed record ResolvedTenant(Guid TenantId, TenantIsolationMode IsolationMode);
