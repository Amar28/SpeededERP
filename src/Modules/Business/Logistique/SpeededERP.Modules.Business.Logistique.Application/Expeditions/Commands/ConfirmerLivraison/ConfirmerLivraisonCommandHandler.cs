using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Business.Logistique.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.Logistique.Application.Expeditions.Commands.ConfirmerLivraison;

public sealed class ConfirmerLivraisonCommandHandler(IExpeditionRepository repository, [FromKeyedServices("logistique")] IUnitOfWork unitOfWork)
    : IRequestHandler<ConfirmerLivraisonCommand, Result>
{
    public async Task<Result> Handle(ConfirmerLivraisonCommand request, CancellationToken cancellationToken)
    {
        var expedition = await repository.GetByIdAsync(request.ExpeditionId, cancellationToken).ConfigureAwait(false);
        if (expedition is null)
        {
            return Result.Failure(Error.NotFound("Expedition.NotFound", "Expédition introuvable."));
        }

        var result = expedition.ConfirmerLivraison();
        if (result.IsFailure)
        {
            return result;
        }

        repository.Update(expedition);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success();
    }
}
