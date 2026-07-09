using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Config.Domain.Repositories;
using SpeededERP.SharedKernel;
using TenantSettingsAggregate = SpeededERP.Modules.Core.Config.Domain.Aggregates.TenantSettings.TenantSettings;

namespace SpeededERP.Modules.Core.Config.Application.TenantSettings.Commands.InitializeTenantSettings;

public sealed class InitializeTenantSettingsCommandHandler(ITenantSettingsRepository repository, [FromKeyedServices("config")] IUnitOfWork unitOfWork)
    : IRequestHandler<InitializeTenantSettingsCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(InitializeTenantSettingsCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByTenantIdAsync(request.TenantId, cancellationToken).ConfigureAwait(false);
        if (existing is not null)
        {
            return Result.Failure<Guid>(Error.Conflict("TenantSettings.AlreadyExists", "Les paramètres de ce tenant existent déjà."));
        }

        var initResult = TenantSettingsAggregate.InitializeForTenant(
            request.TenantId,
            request.DefaultLanguage,
            request.DefaultCurrency,
            request.TimeZoneId);

        if (initResult.IsFailure)
        {
            return Result.Failure<Guid>(initResult.Error);
        }

        repository.Add(initResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(initResult.Value.Id);
    }
}
