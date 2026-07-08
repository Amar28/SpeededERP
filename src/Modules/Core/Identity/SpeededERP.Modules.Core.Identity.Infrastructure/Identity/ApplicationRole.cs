using Microsoft.AspNetCore.Identity;

namespace SpeededERP.Modules.Core.Identity.Infrastructure.Identity;

/// <summary>
/// Technical ASP.NET Core Identity role, used only to satisfy <c>RoleManager</c>
/// plumbing and OpenIddict scope claims. Business roles/permissions are owned by
/// <c>Domain.Aggregates.Roles.Role</c> — this type is not exposed to the Application layer.
/// </summary>
public sealed class ApplicationRole : IdentityRole<Guid>;
