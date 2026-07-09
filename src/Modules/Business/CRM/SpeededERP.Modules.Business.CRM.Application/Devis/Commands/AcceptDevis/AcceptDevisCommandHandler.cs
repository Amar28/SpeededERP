using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Application.Outbox;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Business.CRM.Application.IntegrationEvents;
using SpeededERP.Modules.Business.CRM.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.CRM.Application.Devis.Commands.AcceptDevis;

/// <summary>
/// Reference handler for architecture section 09, Fig. 03: raises the domain event via
/// <c>Devis.Accepter()</c> (in-process reactions only, see DomainEventsInterceptor),
/// and — since this event also matters outside the module — explicitly enqueues the
/// stable <see cref="DevisAcceptedIntegrationEvent"/> contract through the Outbox in the
/// very same unit of work as the aggregate update.
/// </summary>
public sealed class AcceptDevisCommandHandler(
    IDevisRepository repository,
    [FromKeyedServices("crm")] IUnitOfWork unitOfWork,
    [FromKeyedServices("crm")] IOutboxWriter outboxWriter)
    : IRequestHandler<AcceptDevisCommand, Result>
{
    public async Task<Result> Handle(AcceptDevisCommand request, CancellationToken cancellationToken)
    {
        var devis = await repository.GetByIdAsync(request.DevisId, cancellationToken).ConfigureAwait(false);
        if (devis is null)
        {
            return Result.Failure(Error.NotFound("Devis.NotFound", "Devis introuvable."));
        }

        var result = devis.Accepter();
        if (result.IsFailure)
        {
            return result;
        }

        repository.Update(devis);

        await outboxWriter.EnqueueAsync(
            new DevisAcceptedIntegrationEvent
            {
                DevisId = devis.Id,
                ClientId = devis.ClientId,
                Montant = devis.Montant.Amount,
                Devise = devis.Montant.Currency,
                TenantId = devis.TenantId,
            },
            cancellationToken).ConfigureAwait(false);

        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success();
    }
}
