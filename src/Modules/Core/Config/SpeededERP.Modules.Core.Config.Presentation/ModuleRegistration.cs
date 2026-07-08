using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Web.Modules;
using SpeededERP.Modules.Core.Config.Application;
using SpeededERP.Modules.Core.Config.Infrastructure;
using SpeededERP.Modules.Core.Config.Presentation.Endpoints;

namespace SpeededERP.Modules.Core.Config.Presentation;

/// <summary>Composition entry point for Core.Config — per-tenant settings read on nearly every request (architecture section 25).</summary>
public sealed class ModuleRegistration : IModule
{
    public string Name => "Core.Config";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddConfigApplication();
        services.AddConfigInfrastructure(configuration);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapTenantSettingsEndpoints();
    }
}
