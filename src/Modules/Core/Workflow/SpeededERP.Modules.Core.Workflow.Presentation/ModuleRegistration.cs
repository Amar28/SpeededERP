using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Web.Modules;
using SpeededERP.Modules.Core.Workflow.Application;
using SpeededERP.Modules.Core.Workflow.Infrastructure;
using SpeededERP.Modules.Core.Workflow.Presentation.Endpoints;

namespace SpeededERP.Modules.Core.Workflow.Presentation;

/// <summary>
/// Composition entry point for the business-process automation engine (architecture
/// section 12) — scaffolded here with the WorkflowDefinition CRUD surface the Elsa
/// designer/runtime integration will build on.
/// </summary>
public sealed class ModuleRegistration : IModule
{
    public string Name => "Core.Workflow";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddWorkflowApplication();
        services.AddWorkflowInfrastructure(configuration);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapWorkflowDefinitionEndpoints();
    }
}
