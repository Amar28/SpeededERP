using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace SpeededERP.Modules.Core.Workflow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkflowApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}
