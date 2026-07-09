using SpeededERP.BuildingBlocks.Application.Security;

namespace SpeededERP.Worker.Composition;

/// <summary>
/// Worker has no HTTP request to resolve a caller/tenant from — every command it
/// dispatches comes from a MassTransit consumer reacting to another module's
/// integration event (system-to-system, already trusted; never end-user-initiated,
/// so no dispatched command carries a <c>[RequirePermission]</c> attribute today).
/// These satisfy AuthorizationBehavior's constructor dependency without pretending
/// Worker has a real caller identity — see HttpCurrentUserContext/HttpTenantContext
/// (BuildingBlocks.Web) for the Api-side, request-bound equivalents.
/// </summary>
public sealed class SystemCurrentUserContext : ICurrentUserContext
{
    public Guid? UserId => null;

    public Guid TenantId => Guid.Empty;

    public bool IsAuthenticated => false;

    public IReadOnlyCollection<string> Permissions => [];

    public bool HasPermission(string permission) => false;
}

public sealed class SystemTenantContext : ITenantContext
{
    public Guid TenantId => Guid.Empty;

    public TenantIsolationMode IsolationMode => TenantIsolationMode.Shared;
}
