using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Web.Modules;
using SpeededERP.Modules.Core.Tasks.Application;
using SpeededERP.Modules.Core.Tasks.Infrastructure;
using SpeededERP.Modules.Core.Tasks.Presentation.Endpoints;

namespace SpeededERP.Modules.Core.Tasks.Presentation;

/// <summary>Composition entry point for Core.Tasks — the cross-module to-do list (architecture section 20).</summary>
public sealed class ModuleRegistration : IModule
{
    public string Name => "Core.Tasks";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTasksApplication();
        services.AddTasksInfrastructure(configuration);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapTaskEndpoints();
    }
}
