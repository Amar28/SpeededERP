using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Application.Outbox;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.BuildingBlocks.Infrastructure.Outbox;
using SpeededERP.Modules.Business.Comptabilite.Domain.Repositories;
using SpeededERP.Modules.Business.Comptabilite.Infrastructure.Persistence;
using SpeededERP.Modules.Business.Comptabilite.Infrastructure.Persistence.Repositories;

namespace SpeededERP.Modules.Business.Comptabilite.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddComptabiliteInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ComptabiliteDbContext>(options => options
            .UseNpgsql(
                configuration.GetConnectionString("ComptabiliteDb") ?? configuration.GetConnectionString("Default"),
                npgsql => npgsql.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName)));

        services.AddScoped<IOutboxWriter>(sp => sp.GetRequiredService<ComptabiliteDbContext>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ComptabiliteDbContext>());

        services.AddScoped<IFactureRepository, FactureRepository>();

        return services;
    }

    /// <summary>Hosted services — called only from SpeededERP.Worker's Program.cs (see CRM.Infrastructure for the rationale).</summary>
    public static IServiceCollection AddComptabiliteBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<OutboxProcessor<ComptabiliteDbContext>>();

        return services;
    }

    /// <summary>Registers the RabbitMQ consumer endpoints owned by Comptabilité — called from SpeededERP.Worker.</summary>
    public static void ConfigureComptabiliteConsumers(this IBusRegistrationConfigurator configurator)
    {
        // Future: consume DevisAcceptedIntegrationEvent from CRM and LoyerEchu from
        // Immobilier to auto-create invoices — architecture section 09.
    }
}
