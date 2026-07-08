using Microsoft.Extensions.DependencyInjection;

namespace SpeededERP.Modules.Business.RH.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddRhApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}
