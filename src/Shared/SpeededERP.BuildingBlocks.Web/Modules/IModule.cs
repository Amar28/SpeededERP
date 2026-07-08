using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SpeededERP.BuildingBlocks.Web.Modules;

/// <summary>
/// Composition contract every module's Presentation layer implements
/// (<c>ModuleRegistration.cs</c>, section 06/07). SpeededERP.Api discovers and loads
/// one <see cref="IModule"/> per Core/Business module — plus, at runtime, one per
/// installed marketplace plugin (section 28) — without ever referencing the module's
/// internal namespaces directly.
/// </summary>
public interface IModule
{
    /// <summary>Stable technical name, e.g. "CRM", "Core.Metadata" — used in logs, feature flags, plugin manifests.</summary>
    string Name { get; }

    void RegisterServices(IServiceCollection services, IConfiguration configuration);

    void MapEndpoints(IEndpointRouteBuilder endpoints);
}
