using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Application.Outbox;
using SpeededERP.BuildingBlocks.Application.Validation;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.BuildingBlocks.Infrastructure.Outbox;
using SpeededERP.Modules.Core.Metadata.Application.Validation;
using SpeededERP.Modules.Core.Metadata.Domain.Repositories;
using SpeededERP.Modules.Core.Metadata.Infrastructure.Messaging;
using SpeededERP.Modules.Core.Metadata.Infrastructure.Persistence;
using SpeededERP.Modules.Core.Metadata.Infrastructure.Persistence.Repositories;
using SpeededERP.Modules.Core.Metadata.Infrastructure.Validation;

namespace SpeededERP.Modules.Core.Metadata.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMetadataInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MetadataDbContext>(options => options
            .UseNpgsql(
                configuration.GetConnectionString("MetadataDb") ?? configuration.GetConnectionString("Default"),
                npgsql => npgsql.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName)));

        services.AddScoped<IOutboxWriter>(sp => sp.GetRequiredService<MetadataDbContext>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<MetadataDbContext>());

        services.AddScoped<IEntityDefinitionRepository, EntityDefinitionRepository>();
        services.AddScoped<IFieldDefinitionRepository, FieldDefinitionRepository>();
        services.AddScoped<IFormDefinitionRepository, FormDefinitionRepository>();

        services.AddSingleton<IRuleEvaluator, JsonLogicRuleEvaluator>();
        services.AddScoped<ICustomFieldsValidator, SpeededERP.Modules.Core.Metadata.Infrastructure.Validation.CustomFieldsValidator>();

        return services;
    }

    /// <summary>
    /// Hosted services — called only from SpeededERP.Worker's Program.cs (see
    /// Core.Identity.Infrastructure.DependencyInjection for the rationale).
    /// </summary>
    public static IServiceCollection AddMetadataBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<OutboxProcessor<MetadataDbContext>>();

        return services;
    }

    /// <summary>Registers the RabbitMQ consumer endpoints owned by Core.Metadata — called from SpeededERP.Worker.</summary>
    public static void ConfigureMetadataConsumers(this IBusRegistrationConfigurator configurator)
    {
        configurator.AddConsumer<EntityDefinitionDeclaredConsumer>();
    }
}
