using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace SpeededERP.Modules.Core.Config.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddConfigApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}
