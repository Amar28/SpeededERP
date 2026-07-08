using MediatR;
using Microsoft.AspNetCore.Routing;
using SpeededERP.BuildingBlocks.Web.Errors;
using SpeededERP.Modules.Core.Config.Application.TenantSettings.Commands.InitializeTenantSettings;
using SpeededERP.Modules.Core.Config.Application.TenantSettings.Queries.GetTenantSettings;
using SpeededERP.Modules.Core.Config.Presentation.Contracts;

namespace SpeededERP.Modules.Core.Config.Presentation.Endpoints;

public static class TenantSettingsEndpoints
{
    public static IEndpointRouteBuilder MapTenantSettingsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/config/tenant-settings").WithTags("Core.Config.TenantSettings").RequireAuthorization();

        group.MapGet("/{tenantId:guid}", async (Guid tenantId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetTenantSettingsQuery(tenantId), ct).ConfigureAwait(false);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        }).WithName("GetTenantSettings");

        group.MapPost("/", async (InitializeTenantSettingsRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new InitializeTenantSettingsCommand(
                request.TenantId,
                request.DefaultLanguage,
                request.DefaultCurrency,
                request.TimeZoneId);

            var result = await sender.Send(command, ct).ConfigureAwait(false);

            return result.IsSuccess
                ? Results.CreatedAtRoute("GetTenantSettings", new { tenantId = result.Value }, new { tenantId = result.Value })
                : result.ToProblem();
        }).WithName("InitializeTenantSettings");

        return endpoints;
    }
}
