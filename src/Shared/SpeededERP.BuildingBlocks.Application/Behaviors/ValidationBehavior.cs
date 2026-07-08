using FluentValidation;
using MediatR;
using SpeededERP.BuildingBlocks.Application.Messaging;
using SpeededERP.SharedKernel;

namespace SpeededERP.BuildingBlocks.Application.Behaviors;

/// <summary>
/// First stage of the shared pipeline (section 08): runs every registered
/// <see cref="IValidator{T}"/> for the request — both FluentValidation rules coded by
/// the module author and, where applicable, the tenant's configurable rules evaluated
/// through IRuleEvaluator (section 13) inside a dedicated validator. Two sources, one
/// pipeline.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next().ConfigureAwait(false);
        }

        var context = new ValidationContext<TRequest>(request);

        var failures = (await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)))
                .ConfigureAwait(false))
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToList();

        if (failures.Count == 0)
        {
            return await next().ConfigureAwait(false);
        }

        var error = Error.Validation(
            "Validation.Failed",
            string.Join(" | ", failures.Select(f => f.ErrorMessage)));

        return CreateValidationResult<TResponse>(error);
    }

    private static TResult CreateValidationResult<TResult>(Error error)
        where TResult : Result
    {
        if (typeof(TResult) == typeof(Result))
        {
            return (Result.Failure(error) as TResult)!;
        }

        var resultType = typeof(TResult).GetGenericArguments()[0];
        var failureMethod = typeof(Result)
            .GetMethod(nameof(Result.Failure), 1, [typeof(Error)])!
            .MakeGenericMethod(resultType);

        return (TResult)failureMethod.Invoke(null, [error])!;
    }
}
