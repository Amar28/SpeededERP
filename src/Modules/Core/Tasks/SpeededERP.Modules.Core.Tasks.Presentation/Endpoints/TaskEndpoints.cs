using MediatR;
using Microsoft.AspNetCore.Routing;
using SpeededERP.BuildingBlocks.Web.Errors;
using SpeededERP.Modules.Core.Tasks.Application.TaskItems.Commands.CompleteTask;
using SpeededERP.Modules.Core.Tasks.Application.TaskItems.Commands.CreateTask;
using SpeededERP.Modules.Core.Tasks.Application.TaskItems.Queries.GetTaskById;
using SpeededERP.Modules.Core.Tasks.Presentation.Contracts;

namespace SpeededERP.Modules.Core.Tasks.Presentation.Endpoints;

public static class TaskEndpoints
{
    public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/tasks").WithTags("Core.Tasks").RequireAuthorization();

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetTaskByIdQuery(id), ct).ConfigureAwait(false);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        }).WithName("GetTaskById");

        group.MapPost("/", async (CreateTaskRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new CreateTaskCommand(request.Title, request.Description, request.AssigneeUserId, request.DueDateUtc);
            var result = await sender.Send(command, ct).ConfigureAwait(false);

            return result.IsSuccess
                ? Results.CreatedAtRoute("GetTaskById", new { id = result.Value }, new { id = result.Value })
                : result.ToProblem();
        }).WithName("CreateTask");

        group.MapPost("/{id:guid}/complete", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new CompleteTaskCommand(id), ct).ConfigureAwait(false);
            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        }).WithName("CompleteTask");

        return endpoints;
    }
}
