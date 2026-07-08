using MediatR;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Business.Comptabilite.Domain.Aggregates.Factures;
using SpeededERP.Modules.Business.Comptabilite.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.Comptabilite.Application.Factures.Commands.CreerFacture;

public sealed class CreerFactureCommandHandler(IFactureRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreerFactureCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreerFactureCommand request, CancellationToken cancellationToken)
    {
        var montant = Money.Of(request.Montant, request.Devise);

        var factureResult = Facture.Creer(request.ClientId, montant, request.ReferenceExterne);
        if (factureResult.IsFailure)
        {
            return Result.Failure<Guid>(factureResult.Error);
        }

        repository.Add(factureResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(factureResult.Value.Id);
    }
}
