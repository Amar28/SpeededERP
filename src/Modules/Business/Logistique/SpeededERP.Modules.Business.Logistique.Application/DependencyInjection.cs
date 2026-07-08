using Microsoft.Extensions.DependencyInjection;

namespace SpeededERP.Modules.Business.Logistique.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddLogistiqueApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}
