using Microsoft.Extensions.DependencyInjection;

namespace SpeededERP.Modules.Business.Comptabilite.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddComptabiliteApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}
