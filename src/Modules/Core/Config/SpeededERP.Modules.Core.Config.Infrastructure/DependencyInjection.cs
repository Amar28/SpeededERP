using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Application.Outbox;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.BuildingBlocks.Infrastructure.Outbox;
using SpeededERP.Modules.Core.Config.Domain.Repositories;
using SpeededERP.Modules.Core.Config.Infrastructure.Persistence;
using SpeededERP.Modules.Core.Config.Infrastructure.Persistence.Repositories;

namespace SpeededERP.Modules.Core.Config.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddConfigInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ConfigDbContext>(options => options
            .UseNpgsql(
                configuration.GetConnectionString("ConfigDb") ?? configuration.GetConnectionString("Default"),
                npgsql => npgsql.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName)));

        // Keyed by schema — see Core.Identity.Infrastructure.DependencyInjection for
        // why IUnitOfWork/IOutboxWriter can't be plain AddScoped here.
        services.AddKeyedScoped<IOutboxWriter>("config", (sp, _) => sp.GetRequiredService<ConfigDbContext>());
        services.AddKeyedScoped<IUnitOfWork>("config", (sp, _) => sp.GetRequiredService<ConfigDbContext>());

        services.AddScoped<ITenantSettingsRepository, TenantSettingsRepository>();

        return services;
    }

    /// <summary>
    /// Hosted services — called only from SpeededERP.Worker's Program.cs (see
    /// Core.Identity.Infrastructure.DependencyInjection for the rationale).
    /// </summary>
    public static IServiceCollection AddConfigBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<OutboxProcessor<ConfigDbContext>>();

        return services;
    }

    /// <summary>
    /// Registers the RabbitMQ consumer endpoints owned by Core.Config — called from
    /// SpeededERP.Worker. Reacting to Core.Identity's <c>TenantProvisionedIntegrationEvent</c>
    /// to auto-initialize settings on tenant creation is deferred: that event currently
    /// lives in Core.Identity.Application (module-internal), not in a shared contracts
    /// assembly Core.Config could reference — out of scope for this scaffolding pass.
    /// </summary>
    public static void ConfigureConfigConsumers(this IBusRegistrationConfigurator configurator)
    {
        // Aucun consommateur pour Core.Config à ce stade — voir la remarque ci-dessus.
    }
}
