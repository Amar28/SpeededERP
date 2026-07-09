using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Application.Outbox;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.BuildingBlocks.Infrastructure.Outbox;
using SpeededERP.BuildingBlocks.Infrastructure.Persistence;
using SpeededERP.Modules.Business.CRM.Application.Ports;
using SpeededERP.Modules.Business.CRM.Domain.Repositories;
using SpeededERP.Modules.Business.CRM.Infrastructure.Email;
using SpeededERP.Modules.Business.CRM.Infrastructure.Persistence;
using SpeededERP.Modules.Business.CRM.Infrastructure.Persistence.Repositories;
using SpeededERP.Modules.Business.CRM.Infrastructure.Startup;

namespace SpeededERP.Modules.Business.CRM.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCrmInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CrmDbContext>(options => options
            .UseNpgsql(
                configuration.GetConnectionString("CrmDb") ?? configuration.GetConnectionString("Default"),
                npgsql => npgsql.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName)));

        // Keyed by schema — see Core.Identity.Infrastructure.DependencyInjection for
        // why IUnitOfWork/IOutboxWriter can't be plain AddScoped here.
        services.AddKeyedScoped<IOutboxWriter>("crm", (sp, _) => sp.GetRequiredService<CrmDbContext>());
        services.AddKeyedScoped<IUnitOfWork>("crm", (sp, _) => sp.GetRequiredService<CrmDbContext>());

        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IDevisRepository, DevisRepository>();

        services.AddScoped<IEmailSender, SmtpEmailSender>();

        return services;
    }

    /// <summary>
    /// Hosted services — called only from SpeededERP.Worker's Program.cs (see
    /// Core.Identity.Infrastructure.DependencyInjection for the rationale). The
    /// EntityDefinitionSeeder publishes directly via IPublishEndpoint rather than the
    /// Outbox since it isn't tied to a tenant transaction — it only needs to run once
    /// per process start, and Worker is that single, well-known process.
    /// </summary>
    public static IServiceCollection AddCrmBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<MigrationRunner<CrmDbContext>>();
        services.AddHostedService<OutboxProcessor<CrmDbContext>>();
        services.AddHostedService<EntityDefinitionSeeder>();

        return services;
    }

    /// <summary>Registers the RabbitMQ consumer endpoints owned by CRM — called from SpeededERP.Worker.</summary>
    public static void ConfigureCrmConsumers(this IBusRegistrationConfigurator configurator)
    {
        // Consumers are added here as CRM starts reacting to other modules' events
        // (e.g. ColisLivre from Logistique, section 09).
    }
}
