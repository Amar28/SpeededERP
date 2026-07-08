using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.Modules.Business.CRM.Domain.Services;

namespace SpeededERP.Modules.Business.CRM.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCrmApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<TarificationDomainService>();

        return services;
    }
}
