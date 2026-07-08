using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Web.Modules;
using SpeededERP.Modules.Business.Comptabilite.Application;
using SpeededERP.Modules.Business.Comptabilite.Infrastructure;
using SpeededERP.Modules.Business.Comptabilite.Presentation.Endpoints;

namespace SpeededERP.Modules.Business.Comptabilite.Presentation;

/// <summary>Composition entry point for Comptabilité — the invoicing module (architecture section 09, Fig. 03).</summary>
public sealed class ModuleRegistration : IModule
{
    public string Name => "Comptabilite";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddComptabiliteApplication();
        services.AddComptabiliteInfrastructure(configuration);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapFactureEndpoints();
    }
}
