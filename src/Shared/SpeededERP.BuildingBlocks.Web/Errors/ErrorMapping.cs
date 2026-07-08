using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpeededERP.SharedKernel;

namespace SpeededERP.BuildingBlocks.Web.Errors;

/// <summary>
/// Maps a failed <see cref="Result"/> to an RFC 7807 <see cref="ProblemDetails"/> —
/// the single translation point every Minimal API endpoint uses so REST responses stay
/// consistent across all thirteen modules.
/// </summary>
public static class ErrorMapping
{
    public static IResult ToProblem(this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Impossible de convertir un résultat réussi en problème HTTP.");
        }

        var statusCode = result.Error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError,
        };

        return Microsoft.AspNetCore.Http.Results.Problem(
            statusCode: statusCode,
            title: result.Error.Code,
            detail: result.Error.Message,
            extensions: new Dictionary<string, object?> { ["errorType"] = result.Error.Type.ToString() });
    }
}
