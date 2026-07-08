using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Application.Outbox;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.BuildingBlocks.Infrastructure.Outbox;
using SpeededERP.Modules.Core.Tasks.Domain.Repositories;
using SpeededERP.Modules.Core.Tasks.Infrastructure.Persistence;
using SpeededERP.Modules.Core.Tasks.Infrastructure.Persistence.Repositories;

namespace SpeededERP.Modules.Core.Tasks.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTasksInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TasksDbContext>(options => options
            .UseNpgsql(
                configuration.GetConnectionString("TasksDb") ?? configuration.GetConnectionString("Default"),
                npgsql => npgsql.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName)));

        services.AddScoped<IOutboxWriter>(sp => sp.GetRequiredService<TasksDbContext>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<TasksDbContext>());

        services.AddScoped<ITaskItemRepository, TaskItemRepository>();

        return services;
    }

    /// <summary>
    /// Hosted services — called only from SpeededERP.Worker's Program.cs (see
    /// Core.Identity.Infrastructure.DependencyInjection for the rationale).
    /// </summary>
    public static IServiceCollection AddTasksBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<OutboxProcessor<TasksDbContext>>();

        return services;
    }

    /// <summary>Registers the RabbitMQ consumer endpoints owned by Core.Tasks — called from SpeededERP.Worker.</summary>
    public static void ConfigureTasksConsumers(this IBusRegistrationConfigurator configurator)
    {
        // Aucun consommateur pour Core.Tasks à ce stade.
    }
}
