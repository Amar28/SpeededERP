using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenIddict.Abstractions;
using SpeededERP.Modules.Core.Identity.Domain.Aggregates.Roles;
using SpeededERP.Modules.Core.Identity.Domain.Aggregates.Tenants;
using SpeededERP.Modules.Core.Identity.Domain.Aggregates.Users;
using SpeededERP.Modules.Core.Identity.Domain.ValueObjects;
using SpeededERP.Modules.Core.Identity.Infrastructure.Identity;
using SpeededERP.Modules.Core.Identity.Infrastructure.Persistence;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace SpeededERP.Modules.Core.Identity.Infrastructure.Startup;

/// <summary>
/// Development-only bootstrap: seeds one tenant + an Admin role + an admin user so the
/// admin-portal login has an account to sign in with. There is currently no other way
/// to obtain a first account — role creation and role assignment both require an
/// already-authenticated caller (architecture gap tracked separately). Guarded by
/// <see cref="IHostEnvironment.IsDevelopment"/> and by checking that no tenant already
/// exists, so it is a no-op outside local dev and never runs twice.
/// </summary>
public sealed class DevAdminSeeder(
    IServiceScopeFactory scopeFactory,
    IHostEnvironment environment,
    ILogger<DevAdminSeeder> logger) : IHostedService
{
    public const string DefaultEmail = "admin@speedederp.local";
    public const string DefaultPassword = "ChangeMe123!";
    public const string DevClientId = "speeded-erp-spa";

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!environment.IsDevelopment())
        {
            return;
        }

        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityModuleDbContext>();

        // No EF Core migrations exist yet for this module (or any other), and
        // Database.EnsureCreatedAsync() is a no-op once the *database* exists (it only
        // ever creates schema for a brand-new database) — the shared Postgres instance
        // already has an empty speeded_erp database, so this module's tables would
        // otherwise never get created. Creating tables directly targets only this
        // DbContext's model/schema, leaving other modules' schemas untouched.
        // HasTablesAsync() checks the database for *any* table, not this schema's
        // tables specifically, so it false-positives once any module has created its
        // own — attempt creation unconditionally and ignore "already exists" instead.
        var databaseCreator = (RelationalDatabaseCreator)dbContext.Database.GetService<IDatabaseCreator>();
        try
        {
            await databaseCreator.CreateTablesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.DuplicateTable)
        {
            // identity.* tables already exist from a previous run — nothing to do.
        }

        // The Admin/Tenant portals call /connect/token without a client_id (no
        // interactive login/consent screen exists yet to register a public SPA client
        // against — see OpenIddictServerRegistration.AcceptAnonymousClients()). A
        // completely absent client_id still needs a resolvable client for the password
        // grant to succeed, so seed one public dev client here — dev-only, idempotent.
        var applicationManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        if (await applicationManager.FindByClientIdAsync(DevClientId, cancellationToken).ConfigureAwait(false) is null)
        {
            await applicationManager.CreateAsync(
                new OpenIddictApplicationDescriptor
                {
                    ClientId = DevClientId,
                    ClientType = ClientTypes.Public,
                    DisplayName = "SpeededERP Admin/Tenant Portal (dev)",
                    Permissions =
                    {
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.Password,
                        Permissions.GrantTypes.RefreshToken,
                        Permissions.Prefixes.Scope + "openid",
                        Permissions.Prefixes.Scope + "profile",
                        Permissions.Prefixes.Scope + "email",
                        Permissions.Prefixes.Scope + "offline_access",
                        Permissions.Prefixes.Scope + "speeded_erp_api",
                    },
                },
                cancellationToken).ConfigureAwait(false);

            logger.LogWarning("Seeded dev OpenIddict client — client_id: {ClientId}. Development only.", DevClientId);
        }

        if (await dbContext.Tenants.AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        var tenantResult = Tenant.Provision("SpeededERP Dev", "dev", TenantIsolationMode.Shared);
        var emailResult = EmailAddress.Create(DefaultEmail);
        if (tenantResult.IsFailure || emailResult.IsFailure)
        {
            logger.LogError("Dev admin seeding aborted: invalid seed data.");
            return;
        }

        var tenant = tenantResult.Value;
        tenant.Activate();
        dbContext.Tenants.Add(tenant);

        var roleResult = Role.Create("Admin", "Administrateur — accès complet", isSystemRole: true);
        var role = roleResult.Value;
        role.AssignTenant(tenant.Id);
        dbContext.BusinessRoles.Add(role);

        var userId = Guid.NewGuid();
        var userResult = User.Register(userId, emailResult.Value, "Admin", "SpeededERP", tenant.Id);
        var user = userResult.Value;
        user.Activate();
        user.AssignRole(role.Id);
        dbContext.BusinessUsers.Add(user);

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var applicationUser = new ApplicationUser
        {
            Id = userId,
            UserName = DefaultEmail,
            Email = DefaultEmail,
            EmailConfirmed = true,
            TenantId = tenant.Id,
        };

        var identityResult = await userManager.CreateAsync(applicationUser, DefaultPassword).ConfigureAwait(false);
        if (!identityResult.Succeeded)
        {
            logger.LogError(
                "Dev admin seeding failed to create credentials: {Errors}",
                string.Join(" | ", identityResult.Errors.Select(e => e.Description)));
            return;
        }

        logger.LogWarning(
            "Seeded dev admin account — email: {Email}, password: {Password}. Development only, never use in production.",
            DefaultEmail,
            DefaultPassword);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
