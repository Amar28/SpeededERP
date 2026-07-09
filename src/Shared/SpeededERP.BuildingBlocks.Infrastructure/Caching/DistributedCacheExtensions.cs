using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SpeededERP.BuildingBlocks.Infrastructure.Caching;

/// <summary>
/// Backs <see cref="SpeededERP.BuildingBlocks.Application.Behaviors.CachingBehavior{TRequest,TResponse}"/>'s
/// <c>IDistributedCache</c> (architecture section 25) with the shared Redis instance —
/// called once from each host (Api, Worker) since both dispatch MediatR requests
/// through the same pipeline behaviors.
/// </summary>
public static class DistributedCacheExtensions
{
    public static IServiceCollection AddSpeededErpDistributedCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });

        return services;
    }
}
