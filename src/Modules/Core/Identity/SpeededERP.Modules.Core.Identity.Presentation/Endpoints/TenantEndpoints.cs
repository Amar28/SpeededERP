using MediatR;
using Microsoft.AspNetCore.Routing;
using SpeededERP.BuildingBlocks.Web.Errors;
using SpeededERP.Modules.Core.Identity.Application.Tenants.Commands.ProvisionTenant;
using SpeededERP.Modules.Core.Identity.Presentation.Contracts;

namespace SpeededERP.Modules.Core.Identity.Presentation.Endpoints;

public static class TenantEndpoints
{
    public static IEndpointRouteBuilder MapTenantEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/identity/tenants").WithTags("Identity.Tenants");

        group.MapPost("/", async (ProvisionTenantRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender
                .Send(new ProvisionTenantCommand(request.CompanyName, request.Slug, request.IsolationMode), ct)
                .ConfigureAwait(false);

            return result.IsSuccess
                ? Results.Created($"/api/v1/identity/tenants/{result.Value}", new { id = result.Value })
                : result.ToProblem();
        }).WithName("ProvisionTenant").AllowAnonymous();

        return endpoints;
    }
}
