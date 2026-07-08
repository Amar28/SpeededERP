using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Web.Modules;
using SpeededERP.Modules.Core.Metadata.Application;
using SpeededERP.Modules.Core.Metadata.Infrastructure;
using SpeededERP.Modules.Core.Metadata.Presentation.Endpoints;

namespace SpeededERP.Modules.Core.Metadata.Presentation;

/// <summary>
/// Composition entry point for the no-code metadata engine — the platform's real
/// center of gravity per architecture section 01.
/// </summary>
public sealed class ModuleRegistration : IModule
{
    public string Name => "Core.Metadata";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMetadataApplication();
        services.AddMetadataInfrastructure(configuration);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapFieldDefinitionEndpoints();
        endpoints.MapFormDefinitionEndpoints();
    }
}
