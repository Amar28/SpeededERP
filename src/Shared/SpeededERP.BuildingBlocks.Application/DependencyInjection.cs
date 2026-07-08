using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Application.Behaviors;
using SpeededERP.BuildingBlocks.Application.DomainEvents;
using SpeededERP.BuildingBlocks.Domain;

namespace SpeededERP.BuildingBlocks.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the shared MediatR pipeline once, at the Host composition root
    /// (order matters — mirrors section 08):
    /// ExceptionHandling → Logging → Validation → Authorization → Caching.
    /// Individual modules only need to call
    /// <c>cfg.RegisterServicesFromAssembly(...)</c> for their own handlers.
    /// </summary>
    public static IServiceCollection AddSpeededErpApplicationBehaviors(this IServiceCollection services)
    {
        services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));
        services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
        services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(CachingBehavior<,>));

        services.AddScoped<IDomainEventDispatcher, MediatorDomainEventDispatcher>();

        return services;
    }
}
