namespace SpeededERP.BuildingBlocks.Application.Security;

/// <summary>
/// Outbound port giving Application-layer code access to the caller identity resolved
/// upstream (JWT claims, section 29). Implemented in BuildingBlocks.Web from
/// IHttpContextAccessor so the Application layer never depends on ASP.NET Core.
/// </summary>
public interface ICurrentUserContext
{
    Guid? UserId { get; }

    Guid TenantId { get; }

    bool IsAuthenticated { get; }

    IReadOnlyCollection<string> Permissions { get; }

    /// <summary>Atomic <c>ressource:action</c> permission check (section 18).</summary>
    bool HasPermission(string permission);
}
