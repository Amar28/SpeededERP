using Microsoft.AspNetCore.Identity;

namespace SpeededERP.Modules.Core.Identity.Infrastructure.Identity;

/// <summary>
/// ASP.NET Core Identity credential record (password hash, lockout, MFA, security
/// stamp). Shares its <see cref="IdentityUser{TKey}.Id"/> with the business
/// <c>Domain.Aggregates.Users.User</c> aggregate but never carries business state —
/// see the rationale documented on that aggregate.
/// </summary>
public sealed class ApplicationUser : IdentityUser<Guid>
{
    public Guid TenantId { get; set; }
}
