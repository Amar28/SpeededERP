using MediatR;
using Microsoft.AspNetCore.Routing;
using SpeededERP.BuildingBlocks.Web.Errors;
using SpeededERP.Modules.Core.Metadata.Application.FormDefinitions.Commands.PublishFormDefinition;
using SpeededERP.Modules.Core.Metadata.Application.FormDefinitions.Commands.SaveFormDefinition;
using SpeededERP.Modules.Core.Metadata.Application.FormDefinitions.Queries.GetFormDefinition;
using SpeededERP.Modules.Core.Metadata.Presentation.Contracts;

namespace SpeededERP.Modules.Core.Metadata.Presentation.Endpoints;

public static class FormDefinitionEndpoints
{
    public static IEndpointRouteBuilder MapFormDefinitionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/metadata/forms").WithTags("Metadata.FormDefinitions").RequireAuthorization();

        group.MapGet("/{formDefinitionId:guid}", async (Guid formDefinitionId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetFormDefinitionQuery(formDefinitionId), ct).ConfigureAwait(false);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        }).WithName("GetFormDefinition");

        group.MapPost("/", async (SaveFormDefinitionRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(
                new SaveFormDefinitionCommand(request.FormDefinitionId, request.EntityDefinitionId, request.Name, request.LayoutJson),
                ct).ConfigureAwait(false);

            return result.IsSuccess
                ? Results.Ok(new { id = result.Value })
                : result.ToProblem();
        }).WithName("SaveFormDefinition");

        group.MapPost("/{formDefinitionId:guid}/publish", async (Guid formDefinitionId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new PublishFormDefinitionCommand(formDefinitionId), ct).ConfigureAwait(false);
            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        }).WithName("PublishFormDefinition");

        return endpoints;
    }
}
