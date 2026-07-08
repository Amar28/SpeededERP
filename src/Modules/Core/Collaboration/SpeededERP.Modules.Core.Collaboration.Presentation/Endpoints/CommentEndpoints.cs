using MediatR;
using Microsoft.AspNetCore.Routing;
using SpeededERP.BuildingBlocks.Web.Errors;
using SpeededERP.Modules.Core.Collaboration.Application.Comments.Commands.PostComment;
using SpeededERP.Modules.Core.Collaboration.Application.Comments.Queries.GetCommentsByEntity;
using SpeededERP.Modules.Core.Collaboration.Presentation.Contracts;

namespace SpeededERP.Modules.Core.Collaboration.Presentation.Endpoints;

public static class CommentEndpoints
{
    public static IEndpointRouteBuilder MapCommentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/collaboration/comments").WithTags("Collaboration.Comments").RequireAuthorization();

        group.MapGet("/", async (string entityType, Guid entityId, Guid tenantId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender
                .Send(new GetCommentsByEntityQuery(entityType, entityId, tenantId), ct)
                .ConfigureAwait(false);

            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        }).WithName("GetCommentsByEntity");

        group.MapPost("/", async (PostCommentRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(
                new PostCommentCommand(request.EntityType, request.EntityId, request.AuthorUserId, request.Body),
                ct).ConfigureAwait(false);

            return result.IsSuccess
                ? Results.Ok(new { id = result.Value })
                : result.ToProblem();
        }).WithName("PostComment");

        return endpoints;
    }
}
