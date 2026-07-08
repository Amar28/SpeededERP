using MediatR;
using Microsoft.AspNetCore.Routing;
using SpeededERP.BuildingBlocks.Web.Errors;
using SpeededERP.Modules.Core.Identity.Application.Users.Commands.AssignRole;
using SpeededERP.Modules.Core.Identity.Application.Users.Commands.RegisterUser;
using SpeededERP.Modules.Core.Identity.Application.Users.Queries.GetUserById;
using SpeededERP.Modules.Core.Identity.Presentation.Contracts;

namespace SpeededERP.Modules.Core.Identity.Presentation.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/identity/users").WithTags("Identity.Users").RequireAuthorization();

        group.MapGet("/{userId:guid}", async (Guid userId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetUserByIdQuery(userId), ct).ConfigureAwait(false);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        }).WithName("GetUserById");

        group.MapPost("/", async (RegisterUserRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender
                .Send(new RegisterUserCommand(request.Email, request.FirstName, request.LastName, request.InitialPassword), ct)
                .ConfigureAwait(false);

            return result.IsSuccess
                ? Results.CreatedAtRoute("GetUserById", new { userId = result.Value }, new { id = result.Value })
                : result.ToProblem();
        }).WithName("RegisterUser").AllowAnonymous();

        group.MapPost("/{userId:guid}/roles/{roleId:guid}", async (Guid userId, Guid roleId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new AssignRoleCommand(userId, roleId), ct).ConfigureAwait(false);
            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        }).WithName("AssignRoleToUser");

        return endpoints;
    }
}
