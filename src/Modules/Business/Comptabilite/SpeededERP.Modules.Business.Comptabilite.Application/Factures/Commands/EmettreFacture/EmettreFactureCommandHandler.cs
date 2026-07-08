using MediatR;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Business.Comptabilite.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.Comptabilite.Application.Factures.Commands.EmettreFacture;

public sealed class EmettreFactureCommandHandler(IFactureRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<EmettreFactureCommand, Result>
{
    public async Task<Result> Handle(EmettreFactureCommand request, CancellationToken cancellationToken)
    {
        var facture = await repository.GetByIdAsync(request.FactureId, cancellationToken).ConfigureAwait(false);
        if (facture is null)
        {
            return Result.Failure(Error.NotFound("Facture.NotFound", "Facture introuvable."));
        }

        var result = facture.Emettre();
        if (result.IsFailure)
        {
            return result;
        }

        repository.Update(facture);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success();
    }
}
