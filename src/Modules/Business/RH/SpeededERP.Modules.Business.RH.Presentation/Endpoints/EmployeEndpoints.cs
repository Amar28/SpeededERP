using MediatR;
using Microsoft.AspNetCore.Routing;
using SpeededERP.BuildingBlocks.Web.Errors;
using SpeededERP.Modules.Business.RH.Application.Employes.Commands.EmbaucherEmploye;
using SpeededERP.Modules.Business.RH.Application.Employes.Queries.GetEmployeById;
using SpeededERP.Modules.Business.RH.Presentation.Contracts;

namespace SpeededERP.Modules.Business.RH.Presentation.Endpoints;

public static class EmployeEndpoints
{
    public static IEndpointRouteBuilder MapEmployeEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/rh/employes").WithTags("RH.Employes").RequireAuthorization();

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetEmployeByIdQuery(id), ct).ConfigureAwait(false);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        }).WithName("GetEmployeById");

        group.MapPost("/", async (EmbaucherEmployeRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender
                .Send(new EmbaucherEmployeCommand(request.Nom, request.Prenom, request.Email, request.Poste, request.DateEmbaucheUtc), ct)
                .ConfigureAwait(false);

            return result.IsSuccess
                ? Results.CreatedAtRoute("GetEmployeById", new { id = result.Value }, new { id = result.Value })
                : result.ToProblem();
        }).WithName("EmbaucherEmploye");

        return endpoints;
    }
}
