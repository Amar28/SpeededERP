using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Web.Modules;
using SpeededERP.Modules.Business.RH.Application;
using SpeededERP.Modules.Business.RH.Infrastructure;
using SpeededERP.Modules.Business.RH.Presentation.Endpoints;

namespace SpeededERP.Modules.Business.RH.Presentation;

/// <summary>Composition entry point for RH — the human-resources module.</summary>
public sealed class ModuleRegistration : IModule
{
    public string Name => "RH";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddRhApplication();
        services.AddRhInfrastructure(configuration);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapEmployeEndpoints();
    }
}
