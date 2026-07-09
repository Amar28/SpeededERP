using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.Modules.Core.Identity.Infrastructure.Persistence;

namespace SpeededERP.Modules.Core.Identity.Presentation.Auth;

/// <summary>
/// OAuth2/OIDC server (architecture section 29). Authorization Code + PKCE is the flow
/// used by the Admin/Tenant portals (public SPA clients, no client secret); the
/// resource-owner-password grant is kept only for first-party server-to-server tooling
/// and CI smoke tests, never exposed to a browser client.
/// </summary>
public static class OpenIddictServerRegistration
{
    public static IServiceCollection AddOpenIddictServer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenIddict()
            .AddCore(options => options.UseEntityFrameworkCore().UseDbContext<IdentityModuleDbContext>())
            .AddServer(options =>
            {
                options
                    .SetAuthorizationEndpointUris("/connect/authorize")
                    .SetTokenEndpointUris("/connect/token")
                    .SetUserinfoEndpointUris("/connect/userinfo")
                    .SetLogoutEndpointUris("/connect/logout");

                options
                    .AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange()
                    .AllowRefreshTokenFlow()
                    .AllowPasswordFlow(); // dev/CI only — restricted via client configuration, see seed data.

                options.RegisterScopes("openid", "profile", "email", "offline_access", "speeded_erp_api");

                options.SetAccessTokenLifetime(TimeSpan.FromMinutes(15));
                options.SetRefreshTokenLifetime(TimeSpan.FromDays(30));

                var certificatePath = configuration["Auth:SigningCertificatePath"];
                var hasCertificate = !string.IsNullOrWhiteSpace(certificatePath);
                if (hasCertificate)
                {
                    options.AddSigningCertificate(new System.Security.Cryptography.X509Certificates.X509Certificate2(
                        certificatePath!,
                        configuration["Auth:SigningCertificatePassword"]));
                    options.AddEncryptionCertificate(new System.Security.Cryptography.X509Certificates.X509Certificate2(
                        certificatePath!,
                        configuration["Auth:SigningCertificatePassword"]));
                }
                else
                {
                    // True in-memory ephemeral keys — fine for local dev/CI containers,
                    // never for staging/prod (see deploy/k8s secrets). Deliberately NOT
                    // AddDevelopmentSigningCertificate(): that API persists into the OS
                    // X509 certificate store under the current user's home directory,
                    // which fails under a minimal non-root container user with no
                    // writable home (confirmed via `docker run` smoke test).
                    options.AddEphemeralSigningKey();
                    options.AddEphemeralEncryptionKey();
                }

                var aspNetCoreBuilder = options.UseAspNetCore()
                    .EnableAuthorizationEndpointPassthrough()
                    .EnableTokenEndpointPassthrough()
                    .EnableUserinfoEndpointPassthrough()
                    .EnableStatusCodePagesIntegration();

                if (!hasCertificate)
                {
                    // No certificate configured means this is the same local dev/CI
                    // deployment as the ephemeral-keys branch above, where every hop
                    // (gateway, portals) runs over plain HTTP — never for staging/prod.
                    aspNetCoreBuilder.DisableTransportSecurityRequirement();
                }
            })
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        return services;
    }
}
