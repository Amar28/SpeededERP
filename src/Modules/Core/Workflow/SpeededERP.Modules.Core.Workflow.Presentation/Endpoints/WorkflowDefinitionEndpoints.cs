using MediatR;
using Microsoft.AspNetCore.Routing;
using SpeededERP.BuildingBlocks.Web.Errors;
using SpeededERP.Modules.Core.Workflow.Application.WorkflowDefinitions.Commands.CreateWorkflowDefinition;
using SpeededERP.Modules.Core.Workflow.Application.WorkflowDefinitions.Queries.GetWorkflowDefinition;
using SpeededERP.Modules.Core.Workflow.Presentation.Contracts;

namespace SpeededERP.Modules.Core.Workflow.Presentation.Endpoints;

public static class WorkflowDefinitionEndpoints
{
    public static IEndpointRouteBuilder MapWorkflowDefinitionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/workflow/definitions").WithTags("Workflow.Definitions").RequireAuthorization();

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetWorkflowDefinitionByIdQuery(id), ct).ConfigureAwait(false);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        }).WithName("GetWorkflowDefinitionById");

        group.MapPost("/", async (CreateWorkflowDefinitionRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(
                new CreateWorkflowDefinitionCommand(request.Name, request.EntityDefinitionId, request.DefinitionJson),
                ct).ConfigureAwait(false);

            return result.IsSuccess
                ? Results.CreatedAtRoute("GetWorkflowDefinitionById", new { id = result.Value }, new { id = result.Value })
                : result.ToProblem();
        }).WithName("CreateWorkflowDefinition");

        return endpoints;
    }
}
