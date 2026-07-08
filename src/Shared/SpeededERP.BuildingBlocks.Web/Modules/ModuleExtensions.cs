using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SpeededERP.BuildingBlocks.Web.Modules;

public static class ModuleExtensions
{
    /// <summary>Registers every module's services — called once from Host.Api/Program.cs.</summary>
    public static IServiceCollection AddModules(
        this IServiceCollection services,
        IConfiguration configuration,
        IEnumerable<IModule> modules)
    {
        foreach (var module in modules)
        {
            module.RegisterServices(services, configuration);
        }

        return services;
    }

    /// <summary>Maps every module's REST/GraphQL endpoints — called once from Host.Api/Program.cs.</summary>
    public static IEndpointRouteBuilder MapModules(this IEndpointRouteBuilder endpoints, IEnumerable<IModule> modules)
    {
        foreach (var module in modules)
        {
            module.MapEndpoints(endpoints);
        }

        return endpoints;
    }
}
