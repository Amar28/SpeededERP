using Microsoft.AspNetCore.Http;
using SpeededERP.BuildingBlocks.Application.Security;

namespace SpeededERP.BuildingBlocks.Web.Security;

/// <summary>
/// Resolves the tenant from the JWT <c>tenant_id</c> claim, falling back to the
/// <c>X-Tenant-Slug</c> header/sub-domain for pre-authentication endpoints (tenant
/// provisioning, login screen) — architecture section 17.
/// </summary>
public sealed class HttpTenantContext(IHttpContextAccessor httpContextAccessor) : ITenantContext
{
    public Guid TenantId
    {
        get
        {
            var context = httpContextAccessor.HttpContext;
            var claim = context?.User.FindFirst("tenant_id")?.Value;
            if (Guid.TryParse(claim, out var id))
            {
                return id;
            }

            if (context is not null && context.Items.TryGetValue("ResolvedTenantId", out var resolved) && resolved is Guid resolvedId)
            {
                return resolvedId;
            }

            return Guid.Empty;
        }
    }

    public TenantIsolationMode IsolationMode
    {
        get
        {
            var context = httpContextAccessor.HttpContext;
            if (context is not null && context.Items.TryGetValue("TenantIsolationMode", out var mode) && mode is TenantIsolationMode isolationMode)
            {
                return isolationMode;
            }

            return TenantIsolationMode.Shared;
        }
    }
}
