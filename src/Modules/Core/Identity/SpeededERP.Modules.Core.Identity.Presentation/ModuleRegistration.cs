using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Web.Modules;
using SpeededERP.Modules.Core.Identity.Application;
using SpeededERP.Modules.Core.Identity.Infrastructure;
using SpeededERP.Modules.Core.Identity.Presentation.Auth;
using SpeededERP.Modules.Core.Identity.Presentation.Endpoints;

namespace SpeededERP.Modules.Core.Identity.Presentation;

/// <summary>
/// Composition entry point for the Identity module, loaded by SpeededERP.Api at
/// startup (architecture section 06). Wires Domain-less Application, EF/OpenIddict
/// Infrastructure, and this module's own REST + GraphQL surface.
/// </summary>
public sealed class ModuleRegistration : IModule
{
    public string Name => "Core.Identity";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityApplication();
        services.AddIdentityInfrastructure(configuration);
        services.AddOpenIddictServer(configuration);
        services.AddScoped<UserClaimsFactory>();
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapAuthEndpoints();
        endpoints.MapUserEndpoints();
        endpoints.MapTenantEndpoints();
        endpoints.MapRoleEndpoints();
    }
}
