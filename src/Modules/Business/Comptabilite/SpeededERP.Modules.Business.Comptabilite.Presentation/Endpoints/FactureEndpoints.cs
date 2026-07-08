using MediatR;
using Microsoft.AspNetCore.Routing;
using SpeededERP.BuildingBlocks.Web.Errors;
using SpeededERP.Modules.Business.Comptabilite.Application.Factures.Commands.CreerFacture;
using SpeededERP.Modules.Business.Comptabilite.Application.Factures.Commands.EmettreFacture;
using SpeededERP.Modules.Business.Comptabilite.Application.Factures.Queries.GetFactureById;
using SpeededERP.Modules.Business.Comptabilite.Presentation.Contracts;

namespace SpeededERP.Modules.Business.Comptabilite.Presentation.Endpoints;

public static class FactureEndpoints
{
    public static IEndpointRouteBuilder MapFactureEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/comptabilite/factures").WithTags("Comptabilite.Factures").RequireAuthorization();

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetFactureByIdQuery(id), ct).ConfigureAwait(false);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        }).WithName("GetFactureById");

        group.MapPost("/", async (CreerFactureRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender
                .Send(new CreerFactureCommand(request.ClientId, request.Montant, request.Devise, request.ReferenceExterne), ct)
                .ConfigureAwait(false);

            return result.IsSuccess
                ? Results.CreatedAtRoute("GetFactureById", new { id = result.Value }, new { id = result.Value })
                : result.ToProblem();
        }).WithName("CreerFacture");

        group.MapPost("/{id:guid}/emettre", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new EmettreFactureCommand(id), ct).ConfigureAwait(false);
            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        }).WithName("EmettreFacture");

        return endpoints;
    }
}
