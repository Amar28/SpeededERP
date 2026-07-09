using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Application.Outbox;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.BuildingBlocks.Infrastructure.Outbox;
using SpeededERP.Modules.Business.Logistique.Domain.Repositories;
using SpeededERP.Modules.Business.Logistique.Infrastructure.Persistence;
using SpeededERP.Modules.Business.Logistique.Infrastructure.Persistence.Repositories;

namespace SpeededERP.Modules.Business.Logistique.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddLogistiqueInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LogistiqueDbContext>(options => options
            .UseNpgsql(
                configuration.GetConnectionString("LogistiqueDb") ?? configuration.GetConnectionString("Default"),
                npgsql => npgsql.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName)));

        // Keyed by schema — see Core.Identity.Infrastructure.DependencyInjection for
        // why IUnitOfWork/IOutboxWriter can't be plain AddScoped here.
        services.AddKeyedScoped<IOutboxWriter>("logistique", (sp, _) => sp.GetRequiredService<LogistiqueDbContext>());
        services.AddKeyedScoped<IUnitOfWork>("logistique", (sp, _) => sp.GetRequiredService<LogistiqueDbContext>());

        services.AddScoped<IExpeditionRepository, ExpeditionRepository>();

        return services;
    }

    /// <summary>Hosted services — called only from SpeededERP.Worker's Program.cs (see Core.Identity.Infrastructure.DependencyInjection for the rationale).</summary>
    public static IServiceCollection AddLogistiqueBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<OutboxProcessor<LogistiqueDbContext>>();

        return services;
    }

    /// <summary>
    /// Registers the RabbitMQ consumer endpoints owned by Logistique — called from SpeededERP.Worker.
    /// Empty for now: future work will have Logistique consume Stock levels read-only when
    /// preparing an Expedition (architecture section 05).
    /// </summary>
    public static void ConfigureLogistiqueConsumers(this IBusRegistrationConfigurator configurator)
    {
    }
}
