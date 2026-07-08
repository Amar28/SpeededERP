using MediatR;
using SpeededERP.Modules.Core.Config.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Config.Application.TenantSettings.Queries.GetTenantSettings;

public sealed class GetTenantSettingsQueryHandler(ITenantSettingsRepository repository)
    : IRequestHandler<GetTenantSettingsQuery, Result<TenantSettingsDto>>
{
    public async Task<Result<TenantSettingsDto>> Handle(GetTenantSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await repository.GetByTenantIdAsync(request.TenantId, cancellationToken).ConfigureAwait(false);
        if (settings is null)
        {
            return Result.Failure<TenantSettingsDto>(Error.NotFound("TenantSettings.NotFound", "Paramètres de tenant introuvables."));
        }

        return Result.Success(new TenantSettingsDto(
            settings.Id,
            settings.DefaultLanguage,
            settings.DefaultCurrency,
            settings.TimeZoneId,
            settings.ActivatedModules));
    }
}
