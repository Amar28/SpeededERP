using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Application.Outbox;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.BuildingBlocks.Infrastructure.Outbox;
using SpeededERP.BuildingBlocks.Infrastructure.Persistence;
using SpeededERP.Modules.Core.Collaboration.Domain.Repositories;
using SpeededERP.Modules.Core.Collaboration.Infrastructure.Persistence;
using SpeededERP.Modules.Core.Collaboration.Infrastructure.Persistence.Repositories;

namespace SpeededERP.Modules.Core.Collaboration.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCollaborationInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CollaborationDbContext>(options => options
            .UseNpgsql(
                configuration.GetConnectionString("CollaborationDb") ?? configuration.GetConnectionString("Default"),
                npgsql => npgsql.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName)));

        // Keyed by schema — see Core.Identity.Infrastructure.DependencyInjection for
        // why IUnitOfWork/IOutboxWriter can't be plain AddScoped here.
        services.AddKeyedScoped<IOutboxWriter>("collaboration", (sp, _) => sp.GetRequiredService<CollaborationDbContext>());
        services.AddKeyedScoped<IUnitOfWork>("collaboration", (sp, _) => sp.GetRequiredService<CollaborationDbContext>());

        services.AddScoped<ICommentRepository, CommentRepository>();

        return services;
    }

    /// <summary>
    /// Hosted services — called only from SpeededERP.Worker's Program.cs (see
    /// Core.Identity.Infrastructure.DependencyInjection for the rationale).
    /// </summary>
    public static IServiceCollection AddCollaborationBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<MigrationRunner<CollaborationDbContext>>();
        services.AddHostedService<OutboxProcessor<CollaborationDbContext>>();

        return services;
    }

    /// <summary>Registers the RabbitMQ consumer endpoints owned by Core.Collaboration — called from SpeededERP.Worker.</summary>
    public static void ConfigureCollaborationConsumers(this IBusRegistrationConfigurator configurator)
    {
        // No consumers yet — added here as the module grows (e.g. reacting to other
        // modules' integration events to post system-generated activity entries).
    }
}
