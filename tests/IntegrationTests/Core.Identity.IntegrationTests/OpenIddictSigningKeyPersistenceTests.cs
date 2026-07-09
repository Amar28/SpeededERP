using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Server;
using SpeededERP.Modules.Core.Identity.Infrastructure.Persistence;
using SpeededERP.Modules.Core.Identity.Presentation.Auth;
using Xunit;

namespace SpeededERP.Modules.Core.Identity.IntegrationTests;

/// <summary>
/// Exercises OpenIddictServerRegistration's environment-conditional key strategy
/// (architecture section 29): Development keeps ephemeral, in-memory keys regenerated
/// every process start; every other environment loads a persisted X.509 certificate
/// specifically so tokens keep validating across restarts/replicas. Each test builds
/// two independent DI containers back to back — simulating a restart — and checks
/// that distinction holds both ways: a token survives with a configured certificate,
/// and, as a contrast, does not with ephemeral keys.
/// </summary>
public sealed class OpenIddictSigningKeyPersistenceTests : IDisposable
{
    private const string CertificatePassword = "test-password";

    private readonly string _certificatePath;

    public OpenIddictSigningKeyPersistenceTests()
    {
        _certificatePath = Path.Combine(Path.GetTempPath(), $"speeded-erp-test-{Guid.NewGuid():N}.pfx");
        File.WriteAllBytes(_certificatePath, CreateSelfSignedCertificatePfx());
    }

    public void Dispose() => File.Delete(_certificatePath);

    [Fact]
    public async Task Token_issued_with_persisted_certificate_remains_valid_after_service_rebuild()
    {
        var configuration = BuildConfiguration(_certificatePath, CertificatePassword);

        var credentials1 = await BuildServerCredentialsAsync(configuration, "Staging");
        var token = IssueToken(credentials1.Signing, credentials1.Encryption);

        // Simulate a restart: a brand-new container, same certificate configuration.
        var credentials2 = await BuildServerCredentialsAsync(configuration, "Staging");
        var result = await ValidateTokenAsync(token, credentials2.Signing, credentials2.Encryption);

        result.IsValid.Should().BeTrue(
            "the same certificate is loaded again on restart, producing the same key material — {0}",
            result.Exception);
    }

    [Fact]
    public async Task Token_issued_with_ephemeral_development_keys_does_not_survive_a_service_rebuild()
    {
        var configuration = BuildConfiguration(path: null, password: null);

        var credentials1 = await BuildServerCredentialsAsync(configuration, "Development");
        var token = IssueToken(credentials1.Signing, credentials1.Encryption);

        // Simulate a restart: ephemeral keys are randomly regenerated every time.
        var credentials2 = await BuildServerCredentialsAsync(configuration, "Development");
        var result = await ValidateTokenAsync(token, credentials2.Signing, credentials2.Encryption);

        result.IsValid.Should().BeFalse("ephemeral keys are deliberately not persisted across restarts in Development");
    }

    [Fact]
    public async Task Startup_fails_fast_when_no_certificate_is_configured_outside_development()
    {
        var configuration = BuildConfiguration(path: null, password: null);

        var act = () => BuildServerCredentialsAsync(configuration, "Staging");

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*No OpenIddict signing certificate is configured*");
    }

    private static async Task<(SecurityKey Signing, SecurityKey Encryption)> BuildServerCredentialsAsync(
        IConfiguration configuration,
        string environmentName)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IHostEnvironment>(new FakeHostEnvironment(environmentName));
        services.AddDbContext<IdentityModuleDbContext>(options => options
            .UseNpgsql("Host=localhost;Database=speeded_erp_test;Username=test;Password=test")
            .UseOpenIddict());

        services.AddOpenIddictServer(configuration);

        await using var provider = services.BuildServiceProvider();

        // Forces Configure/PostConfigure to run now — the same pipeline ValidateOnStart
        // triggers eagerly at real app startup — so a missing certificate throws here.
        var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>().Value;

        return (options.SigningCredentials[0].Key, options.EncryptionCredentials[0].Key);
    }

    private static string IssueToken(SecurityKey signingKey, SecurityKey encryptionKey) =>
        new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = "https://speeded-erp.test/",
            Audience = "speeded-erp-spa",
            Subject = new ClaimsIdentity([new Claim("sub", "test-user")]),
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256),
            EncryptingCredentials = new EncryptingCredentials(encryptionKey, SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes256CbcHmacSha512),
        });

    private static Task<TokenValidationResult> ValidateTokenAsync(string token, SecurityKey signingKey, SecurityKey encryptionKey) =>
        new JsonWebTokenHandler().ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidIssuer = "https://speeded-erp.test/",
            ValidAudience = "speeded-erp-spa",
            IssuerSigningKey = signingKey,
            TokenDecryptionKey = encryptionKey,
        });

    private static IConfiguration BuildConfiguration(string? path, string? password) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Authentication:OpenIddict:SigningCertificate:Path"] = path,
                ["Authentication:OpenIddict:SigningCertificate:Password"] = password,
            })
            .Build();

    private static byte[] CreateSelfSignedCertificatePfx()
    {
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest("CN=speeded-erp-test", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        using var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(1));

        return certificate.Export(X509ContentType.Pfx, CertificatePassword);
    }

    private sealed class FakeHostEnvironment(string environmentName) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = environmentName;

        public string ApplicationName { get; set; } = "SpeededERP.Tests";

        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;

        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
