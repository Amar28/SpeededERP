using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Web.Modules;
using SpeededERP.Modules.Business.Stock.Application;
using SpeededERP.Modules.Business.Stock.Infrastructure;
using SpeededERP.Modules.Business.Stock.Presentation.Endpoints;

namespace SpeededERP.Modules.Business.Stock.Presentation;

/// <summary>Composition entry point for Stock — the Produit aggregate tracks a single warehouse-agnostic stock counter (architecture section 07).</summary>
public sealed class ModuleRegistration : IModule
{
    public string Name => "Stock";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddStockApplication();
        services.AddStockInfrastructure(configuration);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapProduitEndpoints();
    }
}
