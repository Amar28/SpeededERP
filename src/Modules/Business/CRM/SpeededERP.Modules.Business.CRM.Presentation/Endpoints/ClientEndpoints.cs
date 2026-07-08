using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Routing;
using SpeededERP.BuildingBlocks.Web.Errors;
using SpeededERP.Modules.Business.CRM.Application.Clients.Commands.RegisterClient;
using SpeededERP.Modules.Business.CRM.Presentation.Contracts;

namespace SpeededERP.Modules.Business.CRM.Presentation.Endpoints;

public static class ClientEndpoints
{
    private static readonly JsonElement EmptyCustomFields = JsonDocument.Parse("{}").RootElement;

    public static IEndpointRouteBuilder MapClientEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/crm/clients").WithTags("CRM.Clients").RequireAuthorization();

        group.MapPost("/", async (RegisterClientRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new RegisterClientCommand(
                request.Name,
                request.Street,
                request.City,
                request.PostalCode,
                request.CountryIso2,
                request.Phone,
                request.CustomFields ?? EmptyCustomFields);

            var result = await sender.Send(command, ct).ConfigureAwait(false);

            return result.IsSuccess
                ? Results.Created($"/api/v1/crm/clients/{result.Value}", new { id = result.Value })
                : result.ToProblem();
        }).WithName("RegisterClient");

        return endpoints;
    }
}
