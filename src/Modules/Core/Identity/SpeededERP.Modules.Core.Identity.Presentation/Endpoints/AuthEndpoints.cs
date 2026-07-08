using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using SpeededERP.Modules.Core.Identity.Infrastructure.Identity;
using SpeededERP.Modules.Core.Identity.Presentation.Auth;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace SpeededERP.Modules.Core.Identity.Presentation.Endpoints;

/// <summary>
/// Implements the <c>/connect/token</c> exchange for the grant types enabled in
/// <see cref="OpenIddictServerRegistration"/>. The OpenIddict ASP.NET Core integration
/// validates the protocol envelope; this handler only has to authenticate the resource
/// owner and issue claims — see architecture section 29.
/// </summary>
public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/connect/token", ExchangeAsync).WithTags("Auth").WithName("Token").AllowAnonymous();

        return endpoints;
    }

    private static async Task<IResult> ExchangeAsync(
        HttpContext httpContext,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        UserClaimsFactory claimsFactory,
        CancellationToken cancellationToken)
    {
        var request = httpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("La requête OpenIddict est introuvable.");

        if (request.IsPasswordGrantType())
        {
            var user = await userManager.FindByNameAsync(request.Username!).ConfigureAwait(false);
            if (user is null || !await userManager.CheckPasswordAsync(user, request.Password!).ConfigureAwait(false))
            {
                return Forbid(Errors.InvalidGrant, "Identifiants invalides.");
            }

            if (await userManager.IsLockedOutAsync(user).ConfigureAwait(false))
            {
                return Forbid(Errors.InvalidGrant, "Compte verrouillé.");
            }

            var principal = await claimsFactory.CreateAsync(user, cancellationToken).ConfigureAwait(false);
            return Results.SignIn(principal, authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        if (request.IsRefreshTokenGrantType())
        {
            var authenticateResult = await httpContext
                .AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)
                .ConfigureAwait(false);

            var userId = authenticateResult.Principal?.GetClaim(Claims.Subject);
            var user = userId is not null ? await userManager.FindByIdAsync(userId).ConfigureAwait(false) : null;

            if (user is null || !await signInManager.CanSignInAsync(user).ConfigureAwait(false))
            {
                return Forbid(Errors.InvalidGrant, "Le jeton de rafraîchissement n'est plus valide.");
            }

            var principal = await claimsFactory.CreateAsync(user, cancellationToken).ConfigureAwait(false);
            return Results.SignIn(principal, authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        throw new NotImplementedException("Ce type d'octroi n'est pas pris en charge par ce point de terminaison.");
    }

    private static IResult Forbid(string error, string description) =>
        Results.Forbid(
            new AuthenticationProperties(new Dictionary<string, string?>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = error,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = description,
            }),
            [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]);
}
