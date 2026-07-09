using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.Modules.Business.Comptabilite.Application;
using SpeededERP.Modules.Business.Comptabilite.Infrastructure;
using SpeededERP.Modules.Business.CRM.Application;
using SpeededERP.Modules.Business.CRM.Infrastructure;
using SpeededERP.Modules.Business.Immobilier.Application;
using SpeededERP.Modules.Business.Immobilier.Infrastructure;
using SpeededERP.Modules.Business.Logistique.Application;
using SpeededERP.Modules.Business.Logistique.Infrastructure;
using SpeededERP.Modules.Business.RH.Application;
using SpeededERP.Modules.Business.RH.Infrastructure;
using SpeededERP.Modules.Business.Stock.Application;
using SpeededERP.Modules.Business.Stock.Infrastructure;
using SpeededERP.Modules.Core.Collaboration.Application;
using SpeededERP.Modules.Core.Collaboration.Infrastructure;
using SpeededERP.Modules.Core.Config.Application;
using SpeededERP.Modules.Core.Config.Infrastructure;
using SpeededERP.Modules.Core.Documents.Application;
using SpeededERP.Modules.Core.Documents.Infrastructure;
using SpeededERP.Modules.Core.Identity.Application;
using SpeededERP.Modules.Core.Identity.Infrastructure;
using SpeededERP.Modules.Core.Metadata.Application;
using SpeededERP.Modules.Core.Metadata.Infrastructure;
using SpeededERP.Modules.Core.Tasks.Application;
using SpeededERP.Modules.Core.Tasks.Infrastructure;
using SpeededERP.Modules.Core.Workflow.Application;
using SpeededERP.Modules.Core.Workflow.Infrastructure;

namespace SpeededERP.Worker.Composition;

/// <summary>
/// The Worker-side counterpart to SpeededERP.Api's ModuleCatalog: every module's
/// Application (MediatR handlers/validators — needed by consumers that dispatch
/// commands, e.g. EntityDefinitionDeclaredConsumer), Infrastructure (DbContext,
/// repositories) and background services (Outbox publisher, seeders) plus its
/// MassTransit consumers, all in one place. Unlike Host.Api, the Worker doesn't need
/// the <c>IModule</c>/Presentation abstraction — it never serves HTTP — so it calls
/// each module's Application/Infrastructure extension methods directly (architecture
/// section 06: "SpeededERP.Worker — Consumers MassTransit, jobs Hangfire").
/// </summary>
public static class WorkerModules
{
    public static IServiceCollection AddAllModulesInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Core
        services.AddIdentityApplication();
        services.AddIdentityInfrastructure(configuration).AddIdentityBackgroundServices();
        services.AddMetadataApplication();
        services.AddMetadataInfrastructure(configuration).AddMetadataBackgroundServices();
        services.AddWorkflowApplication();
        services.AddWorkflowInfrastructure(configuration).AddWorkflowBackgroundServices();
        services.AddCollaborationApplication();
        services.AddCollaborationInfrastructure(configuration).AddCollaborationBackgroundServices();
        services.AddDocumentsApplication();
        services.AddDocumentsInfrastructure(configuration).AddDocumentsBackgroundServices();
        services.AddTasksApplication();
        services.AddTasksInfrastructure(configuration).AddTasksBackgroundServices();
        services.AddConfigApplication();
        services.AddConfigInfrastructure(configuration).AddConfigBackgroundServices();

        // Business
        services.AddCrmApplication();
        services.AddCrmInfrastructure(configuration).AddCrmBackgroundServices();
        services.AddComptabiliteApplication();
        services.AddComptabiliteInfrastructure(configuration).AddComptabiliteBackgroundServices();
        services.AddRhApplication();
        services.AddRhInfrastructure(configuration).AddRhBackgroundServices();
        services.AddStockApplication();
        services.AddStockInfrastructure(configuration).AddStockBackgroundServices();
        services.AddLogistiqueApplication();
        services.AddLogistiqueInfrastructure(configuration).AddLogistiqueBackgroundServices();
        services.AddImmobilierApplication();
        services.AddImmobilierInfrastructure(configuration).AddImmobilierBackgroundServices();

        return services;
    }

    public static void ConfigureAllModulesConsumers(this IBusRegistrationConfigurator configurator)
    {
        configurator.ConfigureIdentityConsumers();
        configurator.ConfigureMetadataConsumers();
        configurator.ConfigureWorkflowConsumers();
        configurator.ConfigureCollaborationConsumers();
        configurator.ConfigureDocumentsConsumers();
        configurator.ConfigureTasksConsumers();
        configurator.ConfigureConfigConsumers();

        configurator.ConfigureCrmConsumers();
        configurator.ConfigureComptabiliteConsumers();
        configurator.ConfigureRhConsumers();
        configurator.ConfigureStockConsumers();
        configurator.ConfigureLogistiqueConsumers();
        configurator.ConfigureImmobilierConsumers();
    }
}
