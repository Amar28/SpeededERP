using MediatR;
using SpeededERP.BuildingBlocks.Application.Messaging;
using SpeededERP.BuildingBlocks.Application.Security;
using SpeededERP.SharedKernel;

namespace SpeededERP.BuildingBlocks.Application.Behaviors;

/// <summary>See <see cref="RequirePermissionAttribute"/>.</summary>
public sealed class AuthorizationBehavior<TRequest, TResponse>(ICurrentUserContext currentUser)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requiredPermissions = Attribute
            .GetCustomAttributes(typeof(TRequest), typeof(RequirePermissionAttribute))
            .Cast<RequirePermissionAttribute>()
            .Select(a => a.Permission)
            .ToList();

        if (requiredPermissions.Count == 0)
        {
            return await next().ConfigureAwait(false);
        }

        if (!currentUser.IsAuthenticated)
        {
            return CreateFailure(Error.Unauthorized("Authorization.Unauthenticated", "Authentification requise."));
        }

        var missing = requiredPermissions.FirstOrDefault(p => !currentUser.HasPermission(p));
        if (missing is not null)
        {
            return CreateFailure(Error.Forbidden("Authorization.Forbidden", $"Permission requise : {missing}."));
        }

        return await next().ConfigureAwait(false);
    }

    private static TResponse CreateFailure(Error error)
    {
        if (typeof(TResponse) == typeof(Result))
        {
            return (Result.Failure(error) as TResponse)!;
        }

        var resultType = typeof(TResponse).GetGenericArguments()[0];
        var failureMethod = typeof(Result)
            .GetMethod(nameof(Result.Failure), 1, [typeof(Error)])!
            .MakeGenericMethod(resultType);

        return (TResponse)failureMethod.Invoke(null, [error])!;
    }
}
