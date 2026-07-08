using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Web.Modules;
using SpeededERP.Modules.Core.Documents.Application;
using SpeededERP.Modules.Core.Documents.Infrastructure;
using SpeededERP.Modules.Core.Documents.Presentation.Endpoints;

namespace SpeededERP.Modules.Core.Documents.Presentation;

/// <summary>Composition entry point for Core.Documents — file upload/versioning + attachment metadata (architecture section 21).</summary>
public sealed class ModuleRegistration : IModule
{
    public string Name => "Core.Documents";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDocumentsApplication();
        services.AddDocumentsInfrastructure(configuration);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDocumentEndpoints();
    }
}
