using MediatR;
using Microsoft.AspNetCore.Routing;
using SpeededERP.BuildingBlocks.Web.Errors;
using SpeededERP.Modules.Core.Identity.Application.Roles.Commands.CreateRole;
using SpeededERP.Modules.Core.Identity.Presentation.Contracts;

namespace SpeededERP.Modules.Core.Identity.Presentation.Endpoints;

public static class RoleEndpoints
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/identity/roles").WithTags("Identity.Roles").RequireAuthorization();

        group.MapPost("/", async (CreateRoleRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender
                .Send(new CreateRoleCommand(request.Name, request.Description, request.Permissions), ct)
                .ConfigureAwait(false);

            return result.IsSuccess
                ? Results.Created($"/api/v1/identity/roles/{result.Value}", new { id = result.Value })
                : result.ToProblem();
        }).WithName("CreateRole");

        return endpoints;
    }
}
