using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Web.Modules;
using SpeededERP.Modules.Business.Logistique.Application;
using SpeededERP.Modules.Business.Logistique.Infrastructure;
using SpeededERP.Modules.Business.Logistique.Presentation.Endpoints;

namespace SpeededERP.Modules.Business.Logistique.Presentation;

/// <summary>Composition entry point for Logistique — the Expedition aggregate walks Preparee -&gt; EnTransit -&gt; Livree (architecture section 07).</summary>
public sealed class ModuleRegistration : IModule
{
    public string Name => "Logistique";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogistiqueApplication();
        services.AddLogistiqueInfrastructure(configuration);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapExpeditionEndpoints();
    }
}
