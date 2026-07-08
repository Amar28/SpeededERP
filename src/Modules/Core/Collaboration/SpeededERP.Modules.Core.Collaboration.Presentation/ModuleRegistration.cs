using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Web.Modules;
using SpeededERP.Modules.Core.Collaboration.Application;
using SpeededERP.Modules.Core.Collaboration.Infrastructure;
using SpeededERP.Modules.Core.Collaboration.Presentation.Endpoints;

namespace SpeededERP.Modules.Core.Collaboration.Presentation;

/// <summary>
/// Composition entry point for the cross-module activity feed / commenting engine
/// (architecture section 05, "fil d'activité sur tout enregistrement").
/// </summary>
public sealed class ModuleRegistration : IModule
{
    public string Name => "Core.Collaboration";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddCollaborationApplication();
        services.AddCollaborationInfrastructure(configuration);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapCommentEndpoints();
    }
}
