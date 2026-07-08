using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Application.Security;
using SpeededERP.BuildingBlocks.Web.Security;

namespace SpeededERP.BuildingBlocks.Web;

public static class DependencyInjection
{
    /// <summary>Registers the HTTP-backed ICurrentUserContext/ITenantContext ports — called once from Host.Api.</summary>
    public static IServiceCollection AddSpeededErpWebCore(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserContext, HttpCurrentUserContext>();
        services.AddScoped<ITenantContext, HttpTenantContext>();

        return services;
    }
}
