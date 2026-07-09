using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Application.Outbox;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.BuildingBlocks.Infrastructure.Outbox;
using SpeededERP.Modules.Business.Immobilier.Domain.Repositories;
using SpeededERP.Modules.Business.Immobilier.Infrastructure.Persistence;
using SpeededERP.Modules.Business.Immobilier.Infrastructure.Persistence.Repositories;

namespace SpeededERP.Modules.Business.Immobilier.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddImmobilierInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ImmobilierDbContext>(options => options
            .UseNpgsql(
                configuration.GetConnectionString("ImmobilierDb") ?? configuration.GetConnectionString("Default"),
                npgsql => npgsql.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName)));

        // Keyed by schema — see Core.Identity.Infrastructure.DependencyInjection for
        // why IUnitOfWork/IOutboxWriter can't be plain AddScoped here.
        services.AddKeyedScoped<IOutboxWriter>("immobilier", (sp, _) => sp.GetRequiredService<ImmobilierDbContext>());
        services.AddKeyedScoped<IUnitOfWork>("immobilier", (sp, _) => sp.GetRequiredService<ImmobilierDbContext>());

        services.AddScoped<IBienRepository, BienRepository>();

        return services;
    }

    /// <summary>Hosted services — called only from SpeededERP.Worker's Program.cs (see Core.Identity.Infrastructure.DependencyInjection for the rationale).</summary>
    public static IServiceCollection AddImmobilierBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<OutboxProcessor<ImmobilierDbContext>>();

        return services;
    }

    /// <summary>
    /// Registers the RabbitMQ consumer endpoints owned by Immobilier — called from SpeededERP.Worker.
    /// Empty for now: future work will publish a LoyerEchu integration event to Comptabilité
    /// once rent-due scheduling is implemented (architecture sections 05/09).
    /// </summary>
    public static void ConfigureImmobilierConsumers(this IBusRegistrationConfigurator configurator)
    {
    }
}
