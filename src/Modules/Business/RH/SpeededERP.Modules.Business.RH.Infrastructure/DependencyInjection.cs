using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Application.Outbox;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.BuildingBlocks.Infrastructure.Outbox;
using SpeededERP.BuildingBlocks.Infrastructure.Persistence;
using SpeededERP.Modules.Business.RH.Domain.Repositories;
using SpeededERP.Modules.Business.RH.Infrastructure.Persistence;
using SpeededERP.Modules.Business.RH.Infrastructure.Persistence.Repositories;

namespace SpeededERP.Modules.Business.RH.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddRhInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<RhDbContext>(options => options
            .UseNpgsql(
                configuration.GetConnectionString("RhDb") ?? configuration.GetConnectionString("Default"),
                npgsql => npgsql.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName)));

        // Keyed by schema — see Core.Identity.Infrastructure.DependencyInjection for
        // why IUnitOfWork/IOutboxWriter can't be plain AddScoped here.
        services.AddKeyedScoped<IOutboxWriter>("rh", (sp, _) => sp.GetRequiredService<RhDbContext>());
        services.AddKeyedScoped<IUnitOfWork>("rh", (sp, _) => sp.GetRequiredService<RhDbContext>());

        services.AddScoped<IEmployeRepository, EmployeRepository>();

        return services;
    }

    /// <summary>Hosted services — called only from SpeededERP.Worker's Program.cs (see CRM.Infrastructure for the rationale).</summary>
    public static IServiceCollection AddRhBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<MigrationRunner<RhDbContext>>();
        services.AddHostedService<OutboxProcessor<RhDbContext>>();

        return services;
    }

    /// <summary>Registers the RabbitMQ consumer endpoints owned by RH — called from SpeededERP.Worker.</summary>
    public static void ConfigureRhConsumers(this IBusRegistrationConfigurator configurator)
    {
        // Future: consume events relevant to HR workflows (e.g. onboarding triggers from
        // Core.Identity) as RH starts reacting to other modules — architecture section 09.
    }
}
