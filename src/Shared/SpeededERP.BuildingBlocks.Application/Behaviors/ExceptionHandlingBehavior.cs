using MediatR;
using Microsoft.Extensions.Logging;
using SpeededERP.BuildingBlocks.Application.Messaging;
using SpeededERP.SharedKernel;

namespace SpeededERP.BuildingBlocks.Application.Behaviors;

/// <summary>
/// First link of the pipeline (section 08). Turns any unexpected exception into a
/// generic <see cref="Result"/> failure instead of letting it bubble as a 500 with a
/// stack trace — expected business failures should already be modeled as Result
/// failures by the handler and never reach this behavior as exceptions.
/// </summary>
public sealed class ExceptionHandlingBehavior<TRequest, TResponse>(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next().ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Exception non gérée lors du traitement de {RequestName}", typeof(TRequest).Name);

            var error = Error.Failure("Unhandled.Exception", "Une erreur inattendue s'est produite.");

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
}
