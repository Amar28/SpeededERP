using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Application.Outbox;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.BuildingBlocks.Infrastructure.Outbox;
using SpeededERP.BuildingBlocks.Infrastructure.Persistence;
using SpeededERP.Modules.Core.Documents.Application.Ports;
using SpeededERP.Modules.Core.Documents.Domain.Repositories;
using SpeededERP.Modules.Core.Documents.Infrastructure.Persistence;
using SpeededERP.Modules.Core.Documents.Infrastructure.Persistence.Repositories;
using SpeededERP.Modules.Core.Documents.Infrastructure.Storage;

namespace SpeededERP.Modules.Core.Documents.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDocumentsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DocumentsDbContext>(options => options
            .UseNpgsql(
                configuration.GetConnectionString("DocumentsDb") ?? configuration.GetConnectionString("Default"),
                npgsql => npgsql.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName)));

        // Keyed by schema — see Core.Identity.Infrastructure.DependencyInjection for
        // why IUnitOfWork/IOutboxWriter can't be plain AddScoped here.
        services.AddKeyedScoped<IOutboxWriter>("documents", (sp, _) => sp.GetRequiredService<DocumentsDbContext>());
        services.AddKeyedScoped<IUnitOfWork>("documents", (sp, _) => sp.GetRequiredService<DocumentsDbContext>());

        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddSingleton<IBlobStorage, LocalFileSystemBlobStorage>();

        return services;
    }

    /// <summary>
    /// Hosted services — called only from SpeededERP.Worker's Program.cs (see
    /// Core.Identity.Infrastructure.DependencyInjection for the rationale).
    /// </summary>
    public static IServiceCollection AddDocumentsBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<MigrationRunner<DocumentsDbContext>>();
        services.AddHostedService<OutboxProcessor<DocumentsDbContext>>();

        return services;
    }

    /// <summary>Registers the RabbitMQ consumer endpoints owned by Core.Documents — called from SpeededERP.Worker.</summary>
    public static void ConfigureDocumentsConsumers(this IBusRegistrationConfigurator configurator)
    {
        // Aucun consommateur pour Core.Documents à ce stade — module scaffoldé en attente
        // d'événements entrants (ex. suppression en cascade depuis d'autres modules).
    }
}
