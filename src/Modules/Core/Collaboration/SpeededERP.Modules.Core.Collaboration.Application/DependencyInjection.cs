using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace SpeededERP.Modules.Core.Collaboration.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCollaborationApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}
