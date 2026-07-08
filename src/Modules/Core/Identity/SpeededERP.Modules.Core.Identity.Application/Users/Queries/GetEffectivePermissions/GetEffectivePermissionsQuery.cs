using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Core.Identity.Application.Users.Queries.GetEffectivePermissions;

/// <summary>
/// Union of every permission granted by every role assigned to the user — cached
/// aggressively in Redis (architecture section 18) since it is checked on nearly
/// every request via <see cref="Security.RequirePermissionAttribute"/> and the ASP.NET
/// Core authorization policies.
/// </summary>
public sealed record GetEffectivePermissionsQuery(Guid UserId) : IQuery<IReadOnlyCollection<string>>, ICacheableQuery
{
    public string CacheKey => $"identity:effective-permissions:{UserId}";

    public TimeSpan? Expiration => TimeSpan.FromMinutes(15);
}
