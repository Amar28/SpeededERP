using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;
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

                // Unconditional and always ephemeral here: IHostEnvironment can't be
                // resolved yet at registration time (no container has been built), so
                // the Development-vs-not decision has to happen later, in the
                // PostConfigure below. Non-Development environments discard these in
                // favor of a persisted certificate; Development keeps them as-is.
                options.AddEphemeralSigningKey();
                options.AddEphemeralEncryptionKey();

                options.UseAspNetCore()
                    .EnableAuthorizationEndpointPassthrough()
                    .EnableTokenEndpointPassthrough()
                    .EnableUserinfoEndpointPassthrough()
                    .EnableStatusCodePagesIntegration();
            })
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        // IHostEnvironment is only resolvable from a built container, never during
        // RegisterServices/ConfigureServices itself — calling
        // IServiceCollection.BuildServiceProvider() here would be the ASP0000
        // anti-pattern (a second, orphaned container). PostConfigure defers both
        // environment-dependent decisions below to first options access instead, and
        // ValidateOnStart() forces that access eagerly at app startup rather than
        // lazily on the first request, so a missing certificate fails fast.
        services.AddOptions<OpenIddictServerOptions>()
            .PostConfigure<IHostEnvironment>((options, environment) =>
                ConfigureSigningAndEncryptionKeys(options, configuration, environment))
            .ValidateOnStart();

        services.AddOptions<OpenIddictServerAspNetCoreOptions>()
            .PostConfigure<IHostEnvironment>((options, environment) =>
            {
                if (environment.IsDevelopment())
                {
                    // Every hop (gateway, portals) runs over plain HTTP in local dev/CI
                    // — never for staging/prod (see deploy/k8s secrets and README).
                    options.DisableTransportSecurityRequirement = true;
                }
            });

        return services;
    }

    /// <summary>
    /// Development keeps the ephemeral signing/encryption keys added in
    /// <see cref="AddOpenIddictServer"/> above — a new random RSA key pair every
    /// process start, which is fine locally since nothing needs a token to outlive a
    /// restart. Every other environment instead loads a persisted X.509 certificate
    /// from <c>Authentication:OpenIddict:SigningCertificate</c> (same key material
    /// across restarts/replicas — required for refresh tokens and horizontally-scaled
    /// instances to keep validating each other's tokens; see README "Configuration —
    /// certificat de signature OpenIddict"). Deliberately fails fast — via
    /// <c>ValidateOnStart()</c> above — instead of silently keeping ephemeral keys,
    /// which would work right up until the first restart invalidates every
    /// outstanding token.
    /// </summary>
    private static void ConfigureSigningAndEncryptionKeys(
        OpenIddictServerOptions options,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            return;
        }

        var certificate = LoadCertificate(configuration, environment);

        // Same SigningCredentials/EncryptingCredentials shape OpenIddictServerBuilder's
        // own AddSigningCertificate/AddEncryptionCertificate produce — reimplemented
        // here because IHostEnvironment is only available this late (see above), by
        // which point the builder itself is no longer reachable.
        options.SigningCredentials.Clear();
        options.EncryptionCredentials.Clear();
        options.SigningCredentials.Add(new SigningCredentials(new X509SecurityKey(certificate), SecurityAlgorithms.RsaSha256));
        options.EncryptionCredentials.Add(new EncryptingCredentials(new X509SecurityKey(certificate), SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes256CbcHmacSha512));
    }

    private static X509Certificate2 LoadCertificate(IConfiguration configuration, IHostEnvironment environment)
    {
        var section = configuration.GetSection("Authentication:OpenIddict:SigningCertificate");
        var path = section["Path"];
        var thumbprint = section["Thumbprint"];

        if (!string.IsNullOrWhiteSpace(path))
        {
            // The password is expected to come from a secret store (Kubernetes
            // Secret/env var, user-secrets locally, Key Vault, etc.) — appsettings.json
            // only ever ships an empty placeholder for it, never a real value.
            return new X509Certificate2(path, section["Password"]);
        }

        if (!string.IsNullOrWhiteSpace(thumbprint))
        {
            var storeName = Enum.TryParse<StoreName>(section["StoreName"], out var parsedStoreName) ? parsedStoreName : StoreName.My;
            var storeLocation = Enum.TryParse<StoreLocation>(section["StoreLocation"], out var parsedStoreLocation) ? parsedStoreLocation : StoreLocation.CurrentUser;

            using var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);

            var matches = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: false);
            if (matches.Count == 0)
            {
                throw new InvalidOperationException(
                    $"No certificate with thumbprint '{thumbprint}' was found in the {storeLocation}/{storeName} " +
                    "certificate store (Authentication:OpenIddict:SigningCertificate:Thumbprint).");
            }

            return matches[0];
        }

        throw new InvalidOperationException(
            $"No OpenIddict signing certificate is configured for the '{environment.EnvironmentName}' environment. " +
            "Set 'Authentication:OpenIddict:SigningCertificate:Path' (with ':Password' sourced from a secret store) " +
            "or 'Authentication:OpenIddict:SigningCertificate:Thumbprint' — ephemeral keys are only allowed in " +
            "Development. See README, section \"Configuration — certificat de signature OpenIddict\".");
    }
}
