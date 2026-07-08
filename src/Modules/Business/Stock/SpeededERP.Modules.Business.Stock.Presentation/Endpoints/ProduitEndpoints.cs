using MediatR;
using Microsoft.AspNetCore.Routing;
using SpeededERP.BuildingBlocks.Web.Errors;
using SpeededERP.Modules.Business.Stock.Application.Produits.Commands.CreerProduit;
using SpeededERP.Modules.Business.Stock.Application.Produits.Commands.ReapprovisionnerProduit;
using SpeededERP.Modules.Business.Stock.Application.Produits.Queries.GetProduitById;
using SpeededERP.Modules.Business.Stock.Presentation.Contracts;

namespace SpeededERP.Modules.Business.Stock.Presentation.Endpoints;

public static class ProduitEndpoints
{
    public static IEndpointRouteBuilder MapProduitEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/stock/produits").WithTags("Stock.Produits").RequireAuthorization();

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetProduitByIdQuery(id), ct).ConfigureAwait(false);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        }).WithName("GetProduitById");

        group.MapPost("/", async (CreerProduitRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender
                .Send(new CreerProduitCommand(request.Reference, request.Nom, request.PrixUnitaire, request.Devise, request.SeuilAlerte), ct)
                .ConfigureAwait(false);

            return result.IsSuccess
                ? Results.CreatedAtRoute("GetProduitById", new { id = result.Value }, new { id = result.Value })
                : result.ToProblem();
        }).WithName("CreerProduit");

        group.MapPost("/{id:guid}/reapprovisionner", async (Guid id, ReapprovisionnerRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new ReapprovisionnerCommand(id, request.Quantite), ct).ConfigureAwait(false);
            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        }).WithName("ReapprovisionnerProduit");

        return endpoints;
    }
}
