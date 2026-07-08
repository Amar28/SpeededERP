using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Web.Modules;
using SpeededERP.Modules.Business.CRM.Application;
using SpeededERP.Modules.Business.CRM.Infrastructure;
using SpeededERP.Modules.Business.CRM.Presentation.Endpoints;

namespace SpeededERP.Modules.Business.CRM.Presentation;

/// <summary>
/// Composition entry point for CRM — the reference business module (architecture
/// section 07): Devis walks through the full hexagonal cut + event flow, Client
/// illustrates the native/custom-fields hybrid model (section 10).
/// </summary>
public sealed class ModuleRegistration : IModule
{
    public string Name => "CRM";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddCrmApplication();
        services.AddCrmInfrastructure(configuration);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDevisEndpoints();
        endpoints.MapClientEndpoints();
    }
}
