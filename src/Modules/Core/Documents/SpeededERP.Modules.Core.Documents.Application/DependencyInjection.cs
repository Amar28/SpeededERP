using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace SpeededERP.Modules.Core.Documents.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddDocumentsApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}
