using Microsoft.Extensions.DependencyInjection;

namespace SpeededERP.Modules.Business.Immobilier.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddImmobilierApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}
