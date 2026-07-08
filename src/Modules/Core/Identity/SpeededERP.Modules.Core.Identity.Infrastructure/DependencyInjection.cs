using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.EntityFrameworkCore;
using SpeededERP.BuildingBlocks.Application.Outbox;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.BuildingBlocks.Infrastructure.Outbox;
using SpeededERP.Modules.Core.Identity.Application.Users;
using SpeededERP.Modules.Core.Identity.Domain.Repositories;
using SpeededERP.Modules.Core.Identity.Infrastructure.Identity;
using SpeededERP.Modules.Core.Identity.Infrastructure.Persistence;
using SpeededERP.Modules.Core.Identity.Infrastructure.Persistence.Repositories;

namespace SpeededERP.Modules.Core.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityModuleDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("IdentityDb") ?? configuration.GetConnectionString("Default"),
                npgsql => npgsql.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName));

            // OpenIddict stores its applications/scopes/tokens/authorizations in this
            // same DbContext — one schema per module holds both RBAC and OAuth2 state.
            options.UseOpenIddict();
        });

        services.AddScoped<IOutboxWriter>(sp => sp.GetRequiredService<IdentityModuleDbContext>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<IdentityModuleDbContext>());

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 12;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<IdentityModuleDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserCredentialStore, UserCredentialStore>();

        return services;
    }

    /// <summary>
    /// Hosted services (Outbox publisher, seeders) — called only from
    /// SpeededERP.Worker's Program.cs, never from Host.Api. Running the Outbox
    /// processor in more than one process per module would race on the same rows.
    /// </summary>
    public static IServiceCollection AddIdentityBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<OutboxProcessor<IdentityModuleDbContext>>();

        return services;
    }

    /// <summary>Registers the RabbitMQ consumer endpoints owned by Core.Identity — called from SpeededERP.Worker.</summary>
    public static void ConfigureIdentityConsumers(this IBusRegistrationConfigurator configurator)
    {
        // Consumers are added here as the module grows (e.g. reacting to other modules'
        // integration events to provision default RBAC roles).
    }
}
