using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SpeededERP.BuildingBlocks.Application.Security;

namespace SpeededERP.BuildingBlocks.Web.Security;

/// <summary>
/// Reads the caller identity from the current request's JWT claims (populated by
/// OpenIddict, architecture section 29). The Application layer only sees the
/// <see cref="ICurrentUserContext"/> port — never <see cref="HttpContext"/> directly.
/// </summary>
public sealed class HttpCurrentUserContext(IHttpContextAccessor httpContextAccessor) : ICurrentUserContext
{
    private ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var value = Principal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? Principal?.FindFirstValue("sub");
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public Guid TenantId
    {
        get
        {
            var value = Principal?.FindFirstValue("tenant_id");
            return Guid.TryParse(value, out var id) ? id : Guid.Empty;
        }
    }

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    public IReadOnlyCollection<string> Permissions =>
        Principal?.FindAll("permission").Select(c => c.Value).ToList() ?? [];

    public bool HasPermission(string permission) =>
        Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
}
