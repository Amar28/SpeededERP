using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace SpeededERP.Modules.Core.Tasks.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddTasksApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}
