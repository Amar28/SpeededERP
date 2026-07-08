using MediatR;
using Microsoft.AspNetCore.Routing;
using SpeededERP.BuildingBlocks.Web.Errors;
using SpeededERP.Modules.Core.Documents.Application.Documents.Commands.UploadDocument;
using SpeededERP.Modules.Core.Documents.Application.Documents.Queries.GetDocumentById;
using SpeededERP.Modules.Core.Documents.Presentation.Contracts;

namespace SpeededERP.Modules.Core.Documents.Presentation.Endpoints;

public static class DocumentEndpoints
{
    public static IEndpointRouteBuilder MapDocumentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/documents").WithTags("Core.Documents").RequireAuthorization();

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetDocumentByIdQuery(id), ct).ConfigureAwait(false);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        }).WithName("GetDocumentById");

        group.MapPost("/", async (UploadDocumentRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new UploadDocumentCommand(
                request.FileName,
                request.StorageKey,
                request.ContentType,
                request.SizeBytes,
                request.EntityType,
                request.EntityId);

            var result = await sender.Send(command, ct).ConfigureAwait(false);

            return result.IsSuccess
                ? Results.CreatedAtRoute("GetDocumentById", new { id = result.Value }, new { id = result.Value })
                : result.ToProblem();
        }).WithName("UploadDocument");

        return endpoints;
    }
}
