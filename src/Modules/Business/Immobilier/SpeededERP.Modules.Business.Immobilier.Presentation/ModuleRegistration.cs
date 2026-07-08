using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Web.Modules;
using SpeededERP.Modules.Business.Immobilier.Application;
using SpeededERP.Modules.Business.Immobilier.Infrastructure;
using SpeededERP.Modules.Business.Immobilier.Presentation.Endpoints;

namespace SpeededERP.Modules.Business.Immobilier.Presentation;

/// <summary>Composition entry point for Immobilier — the Bien aggregate tracks a rentable property's occupation state (architecture section 07).</summary>
public sealed class ModuleRegistration : IModule
{
    public string Name => "Immobilier";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddImmobilierApplication();
        services.AddImmobilierInfrastructure(configuration);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapBienEndpoints();
    }
}
