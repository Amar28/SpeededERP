using MediatR;
using Microsoft.AspNetCore.Routing;
using SpeededERP.BuildingBlocks.Web.Errors;
using SpeededERP.Modules.Business.CRM.Application.Devis.Commands.AcceptDevis;
using SpeededERP.Modules.Business.CRM.Application.Devis.Commands.AddLigneDevis;
using SpeededERP.Modules.Business.CRM.Application.Devis.Commands.CreateDevis;
using SpeededERP.Modules.Business.CRM.Application.Devis.Commands.SendDevis;
using SpeededERP.Modules.Business.CRM.Application.Devis.Queries.GetDevisById;
using SpeededERP.Modules.Business.CRM.Presentation.Contracts;

namespace SpeededERP.Modules.Business.CRM.Presentation.Endpoints;

public static class DevisEndpoints
{
    public static IEndpointRouteBuilder MapDevisEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/crm/devis").WithTags("CRM.Devis").RequireAuthorization();

        group.MapGet("/{devisId:guid}", async (Guid devisId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetDevisByIdQuery(devisId), ct).ConfigureAwait(false);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        }).WithName("GetDevisById");

        group.MapPost("/", async (CreateDevisRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new CreateDevisCommand(request.ClientId, request.Devise), ct).ConfigureAwait(false);
            return result.IsSuccess
                ? Results.CreatedAtRoute("GetDevisById", new { devisId = result.Value }, new { id = result.Value })
                : result.ToProblem();
        }).WithName("CreateDevis");

        group.MapPost("/{devisId:guid}/lignes", async (Guid devisId, AddLigneDevisRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender
                .Send(new AddLigneDevisCommand(devisId, request.Description, request.Quantite, request.PrixUnitaire), ct)
                .ConfigureAwait(false);

            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        }).WithName("AddLigneDevis");

        group.MapPost("/{devisId:guid}/envoyer", async (Guid devisId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new SendDevisCommand(devisId), ct).ConfigureAwait(false);
            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        }).WithName("SendDevis");

        // The reference endpoint for architecture section 09, Fig. 03 — accepting a
        // Devis here is what ultimately creates an invoice in Comptabilité, downstream,
        // asynchronously, through the Outbox → RabbitMQ → consumer chain.
        group.MapPost("/{devisId:guid}/accepter", async (Guid devisId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new AcceptDevisCommand(devisId), ct).ConfigureAwait(false);
            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        }).WithName("AcceptDevis");

        return endpoints;
    }
}
