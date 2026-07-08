using MediatR;
using SpeededERP.SharedKernel;

namespace SpeededERP.BuildingBlocks.Application.Messaging;

/// <summary>A read model query — never touches the write-side aggregates directly.</summary>
public interface IQuery<TResponse> : IRequest<Result<TResponse>>;

/// <summary>
/// Marks a query as safe to cache. The <c>CachingBehavior</c> reads/writes the response
/// under <see cref="CacheKey"/> in Redis for <see cref="Expiration"/> — used heavily for
/// FormDefinition / MenuDefinition / WorkflowDefinition reads (architecture section 25).
/// </summary>
public interface ICacheableQuery
{
    string CacheKey { get; }

    TimeSpan? Expiration { get; }
}
