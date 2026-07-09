using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Application.Outbox;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.BuildingBlocks.Infrastructure.Outbox;
using SpeededERP.Modules.Core.Workflow.Domain.Repositories;
using SpeededERP.Modules.Core.Workflow.Infrastructure.Persistence;
using SpeededERP.Modules.Core.Workflow.Infrastructure.Persistence.Repositories;

namespace SpeededERP.Modules.Core.Workflow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkflowInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WorkflowDbContext>(options => options
            .UseNpgsql(
                configuration.GetConnectionString("WorkflowDb") ?? configuration.GetConnectionString("Default"),
                npgsql => npgsql.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName)));

        // Keyed by schema — see Core.Identity.Infrastructure.DependencyInjection for
        // why IUnitOfWork/IOutboxWriter can't be plain AddScoped here.
        services.AddKeyedScoped<IOutboxWriter>("workflow", (sp, _) => sp.GetRequiredService<WorkflowDbContext>());
        services.AddKeyedScoped<IUnitOfWork>("workflow", (sp, _) => sp.GetRequiredService<WorkflowDbContext>());

        services.AddScoped<IWorkflowDefinitionRepository, WorkflowDefinitionRepository>();

        return services;
    }

    /// <summary>
    /// Hosted services — called only from SpeededERP.Worker's Program.cs (see
    /// Core.Identity.Infrastructure.DependencyInjection for the rationale).
    /// </summary>
    public static IServiceCollection AddWorkflowBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<OutboxProcessor<WorkflowDbContext>>();

        return services;
    }

    /// <summary>Registers the RabbitMQ consumer endpoints owned by Core.Workflow — called from SpeededERP.Worker.</summary>
    public static void ConfigureWorkflowConsumers(this IBusRegistrationConfigurator configurator)
    {
        // No consumers yet — added here as the module grows (e.g. reacting to metadata
        // record lifecycle events to trigger a workflow run).
    }
}
