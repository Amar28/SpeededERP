using MediatR;
using Microsoft.AspNetCore.Routing;
using SpeededERP.BuildingBlocks.Application.Security;
using SpeededERP.BuildingBlocks.Web.Errors;
using SpeededERP.Modules.Core.Metadata.Application.FieldDefinitions.Commands.AddFieldDefinition;
using SpeededERP.Modules.Core.Metadata.Application.FieldDefinitions.Queries.GetFieldDefinitionsByEntity;
using SpeededERP.Modules.Core.Metadata.Presentation.Contracts;

namespace SpeededERP.Modules.Core.Metadata.Presentation.Endpoints;

public static class FieldDefinitionEndpoints
{
    public static IEndpointRouteBuilder MapFieldDefinitionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/metadata/entities/{entityDefinitionId:guid}/fields")
            .WithTags("Metadata.FieldDefinitions")
            .RequireAuthorization();

        group.MapGet("/", async (Guid entityDefinitionId, ITenantContext tenant, ISender sender, CancellationToken ct) =>
        {
            var result = await sender
                .Send(new GetFieldDefinitionsByEntityQuery(entityDefinitionId, tenant.TenantId), ct)
                .ConfigureAwait(false);

            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        }).WithName("GetFieldDefinitionsByEntity");

        group.MapPost("/", async (Guid entityDefinitionId, AddFieldDefinitionRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(
                new AddFieldDefinitionCommand(
                    entityDefinitionId,
                    request.Key,
                    request.LabelKey,
                    request.Type,
                    request.IsRequired,
                    request.Order,
                    request.ListOptions,
                    request.ValidationRuleJson),
                ct).ConfigureAwait(false);

            return result.IsSuccess
                ? Results.Created($"/api/v1/metadata/entities/{entityDefinitionId}/fields/{result.Value}", new { id = result.Value })
                : result.ToProblem();
        }).WithName("AddFieldDefinition");

        return endpoints;
    }
}
