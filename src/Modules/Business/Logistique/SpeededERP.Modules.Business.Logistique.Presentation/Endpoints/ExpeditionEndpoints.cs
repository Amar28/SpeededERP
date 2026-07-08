using MediatR;
using Microsoft.AspNetCore.Routing;
using SpeededERP.BuildingBlocks.Web.Errors;
using SpeededERP.Modules.Business.Logistique.Application.Expeditions.Commands.ConfirmerLivraison;
using SpeededERP.Modules.Business.Logistique.Application.Expeditions.Commands.PreparerExpedition;
using SpeededERP.Modules.Business.Logistique.Application.Expeditions.Queries.GetExpeditionById;
using SpeededERP.Modules.Business.Logistique.Presentation.Contracts;

namespace SpeededERP.Modules.Business.Logistique.Presentation.Endpoints;

public static class ExpeditionEndpoints
{
    public static IEndpointRouteBuilder MapExpeditionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/logistique/expeditions").WithTags("Logistique.Expeditions").RequireAuthorization();

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetExpeditionByIdQuery(id), ct).ConfigureAwait(false);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        }).WithName("GetExpeditionById");

        group.MapPost("/", async (PreparerExpeditionRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender
                .Send(
                    new PreparerExpeditionCommand(
                        request.NumeroSuivi,
                        request.TransporteurNom,
                        request.Street,
                        request.City,
                        request.PostalCode,
                        request.CountryIso2),
                    ct)
                .ConfigureAwait(false);

            return result.IsSuccess
                ? Results.CreatedAtRoute("GetExpeditionById", new { id = result.Value }, new { id = result.Value })
                : result.ToProblem();
        }).WithName("PreparerExpedition");

        group.MapPost("/{id:guid}/confirmer-livraison", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new ConfirmerLivraisonCommand(id), ct).ConfigureAwait(false);
            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        }).WithName("ConfirmerLivraison");

        return endpoints;
    }
}
