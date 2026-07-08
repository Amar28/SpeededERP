using MediatR;
using Microsoft.AspNetCore.Routing;
using SpeededERP.BuildingBlocks.Web.Errors;
using SpeededERP.Modules.Business.Immobilier.Application.Biens.Commands.EnregistrerBien;
using SpeededERP.Modules.Business.Immobilier.Application.Biens.Commands.MettreEnLocation;
using SpeededERP.Modules.Business.Immobilier.Application.Biens.Queries.GetBienById;
using SpeededERP.Modules.Business.Immobilier.Presentation.Contracts;

namespace SpeededERP.Modules.Business.Immobilier.Presentation.Endpoints;

public static class BienEndpoints
{
    public static IEndpointRouteBuilder MapBienEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/immobilier/biens").WithTags("Immobilier.Biens").RequireAuthorization();

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetBienByIdQuery(id), ct).ConfigureAwait(false);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        }).WithName("GetBienById");

        group.MapPost("/", async (EnregistrerBienRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender
                .Send(
                    new EnregistrerBienCommand(
                        request.Street,
                        request.City,
                        request.PostalCode,
                        request.CountryIso2,
                        request.TypeBien,
                        request.LoyerMensuel,
                        request.Devise),
                    ct)
                .ConfigureAwait(false);

            return result.IsSuccess
                ? Results.CreatedAtRoute("GetBienById", new { id = result.Value }, new { id = result.Value })
                : result.ToProblem();
        }).WithName("EnregistrerBien");

        group.MapPost("/{id:guid}/louer", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new MettreEnLocationCommand(id), ct).ConfigureAwait(false);
            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        }).WithName("MettreBienEnLocation");

        return endpoints;
    }
}
