using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using SpeededERP.Modules.Core.Identity.Domain.Repositories;
using SpeededERP.Modules.Core.Identity.Infrastructure.Identity;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace SpeededERP.Modules.Core.Identity.Presentation.Auth;

/// <summary>
/// Builds the JWT claims principal for an authenticated user: identity claims from
/// ASP.NET Core Identity plus <c>tenant_id</c> and the flattened <c>permission</c>
/// claims resolved from the business User/Role aggregates (architecture section 18) —
/// kept in this module since only Core.Identity may read its own aggregates directly.
/// </summary>
public sealed class UserClaimsFactory(
    IUserRepository userRepository,
    IRoleRepository roleRepository)
{
    public async Task<ClaimsPrincipal> CreateAsync(ApplicationUser applicationUser, CancellationToken cancellationToken = default)
    {
        var identity = new ClaimsIdentity(AuthenticationType, Claims.Name, Claims.Role);

        identity.AddClaim(new Claim(Claims.Subject, applicationUser.Id.ToString()));
        identity.AddClaim(new Claim(Claims.Email, applicationUser.Email ?? string.Empty));
        identity.AddClaim(new Claim("tenant_id", applicationUser.TenantId.ToString()));

        var businessUser = await userRepository.GetByIdAsync(applicationUser.Id, cancellationToken).ConfigureAwait(false);
        if (businessUser is not null)
        {
            identity.AddClaim(new Claim(Claims.Name, $"{businessUser.FirstName} {businessUser.LastName}"));

            var roles = await roleRepository.GetByIdsAsync(businessUser.RoleIds, cancellationToken).ConfigureAwait(false);
            foreach (var permission in roles.SelectMany(r => r.Permissions).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                identity.AddClaim(new Claim("permission", permission));
            }
        }

        var principal = new ClaimsPrincipal(identity);

        principal.SetScopes(Scopes.OpenId, Scopes.Profile, Scopes.Email, Scopes.OfflineAccess, "speeded_erp_api");
        principal.SetResources("speeded_erp_api");

        foreach (var claim in identity.Claims)
        {
            claim.SetDestinations(GetDestinations(claim));
        }

        return principal;
    }

    private const string AuthenticationType = "SpeededERP.OpenIddict";

    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        yield return Destinations.AccessToken;

        if (claim.Type is Claims.Name or Claims.Email or "tenant_id")
        {
            yield return Destinations.IdentityToken;
        }
    }
}
