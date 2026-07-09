using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Application.Outbox;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Identity.Application.IntegrationEvents;
using SpeededERP.Modules.Core.Identity.Domain.Aggregates.Tenants;
using SpeededERP.Modules.Core.Identity.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Identity.Application.Tenants.Commands.ProvisionTenant;

public sealed class ProvisionTenantCommandHandler(
    ITenantRepository tenantRepository,
    [FromKeyedServices("identity")] IUnitOfWork unitOfWork,
    [FromKeyedServices("identity")] IOutboxWriter outboxWriter)
    : IRequestHandler<ProvisionTenantCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(ProvisionTenantCommand request, CancellationToken cancellationToken)
    {
        if (await tenantRepository.SlugExistsAsync(request.Slug, cancellationToken).ConfigureAwait(false))
        {
            return Result.Failure<Guid>(Error.Conflict("Tenant.SlugAlreadyUsed", "Ce nom de domaine est déjà utilisé."));
        }

        var tenantResult = Tenant.Provision(request.CompanyName, request.Slug, request.IsolationMode);
        if (tenantResult.IsFailure)
        {
            return Result.Failure<Guid>(tenantResult.Error);
        }

        var tenant = tenantResult.Value;
        tenantRepository.Add(tenant);

        await outboxWriter.EnqueueAsync(
            new TenantProvisionedIntegrationEvent
            {
                CompanyName = tenant.CompanyName,
                Slug = tenant.Slug,
                TenantId = tenant.Id,
            },
            cancellationToken).ConfigureAwait(false);

        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(tenant.Id);
    }
}
