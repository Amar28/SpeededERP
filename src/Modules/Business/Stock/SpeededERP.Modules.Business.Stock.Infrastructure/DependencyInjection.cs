using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Application.Outbox;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.BuildingBlocks.Infrastructure.Outbox;
using SpeededERP.Modules.Business.Stock.Domain.Repositories;
using SpeededERP.Modules.Business.Stock.Infrastructure.Persistence;
using SpeededERP.Modules.Business.Stock.Infrastructure.Persistence.Repositories;

namespace SpeededERP.Modules.Business.Stock.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddStockInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<StockDbContext>(options => options
            .UseNpgsql(
                configuration.GetConnectionString("StockDb") ?? configuration.GetConnectionString("Default"),
                npgsql => npgsql.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName)));

        services.AddScoped<IOutboxWriter>(sp => sp.GetRequiredService<StockDbContext>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<StockDbContext>());

        services.AddScoped<IProduitRepository, ProduitRepository>();

        return services;
    }

    /// <summary>Hosted services — called only from SpeededERP.Worker's Program.cs (see Core.Identity.Infrastructure.DependencyInjection for the rationale).</summary>
    public static IServiceCollection AddStockBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<OutboxProcessor<StockDbContext>>();

        return services;
    }

    /// <summary>
    /// Registers the RabbitMQ consumer endpoints owned by Stock — called from SpeededERP.Worker.
    /// Empty for now: per architecture section 05, Logistique consults Stock read-only for
    /// warehouse levels rather than Stock consuming events from other modules.
    /// </summary>
    public static void ConfigureStockConsumers(this IBusRegistrationConfigurator configurator)
    {
    }
}
