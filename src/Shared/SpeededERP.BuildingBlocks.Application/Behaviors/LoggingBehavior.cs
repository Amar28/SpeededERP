using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using SpeededERP.SharedKernel;

namespace SpeededERP.BuildingBlocks.Application.Behaviors;

/// <summary>
/// Structured start/outcome/duration logging around every request — enriched with
/// tenant/correlation-id by Serilog at the Host level (section 29).
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        logger.LogInformation("Traitement de {RequestName} démarré", requestName);

        var response = await next().ConfigureAwait(false);

        stopwatch.Stop();

        if (response is Result { IsFailure: true } result)
        {
            logger.LogWarning(
                "{RequestName} échoué en {ElapsedMilliseconds} ms — {ErrorCode}: {ErrorMessage}",
                requestName,
                stopwatch.ElapsedMilliseconds,
                result.Error.Code,
                result.Error.Message);
        }
        else
        {
            logger.LogInformation(
                "{RequestName} terminé en {ElapsedMilliseconds} ms",
                requestName,
                stopwatch.ElapsedMilliseconds);
        }

        return response;
    }
}
