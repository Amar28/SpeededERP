using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SpeededERP.BuildingBlocks.Application.Messaging;
using SpeededERP.SharedKernel;

namespace SpeededERP.BuildingBlocks.Application.Behaviors;

/// <summary>
/// Cache-aside for queries opting in via <see cref="ICacheableQuery"/> — used
/// aggressively for FormDefinition/WorkflowDefinition/MenuDefinition reads
/// (architecture section 25), invalidated by event on write.
/// </summary>
public sealed class CachingBehavior<TRequest, TResponse>(
    IDistributedCache cache,
    ILogger<CachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ICacheableQuery cacheable)
        {
            return await next().ConfigureAwait(false);
        }

        var cached = await cache.GetStringAsync(cacheable.CacheKey, cancellationToken).ConfigureAwait(false);
        if (cached is not null)
        {
            logger.LogDebug("Cache hit pour {CacheKey}", cacheable.CacheKey);
            return JsonSerializer.Deserialize<TResponse>(cached)!;
        }

        var response = await next().ConfigureAwait(false);

        if (response.IsSuccess)
        {
            var options = new DistributedCacheEntryOptions();
            if (cacheable.Expiration is { } expiration)
            {
                options.SetAbsoluteExpiration(expiration);
            }

            await cache.SetStringAsync(
                cacheable.CacheKey,
                JsonSerializer.Serialize(response),
                options,
                cancellationToken).ConfigureAwait(false);
        }

        return response;
    }
}
