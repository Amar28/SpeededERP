using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.Modules.Core.Metadata.Application.Validation;

namespace SpeededERP.Modules.Core.Metadata.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddMetadataApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<FieldValueValidator>();

        return services;
    }
}
