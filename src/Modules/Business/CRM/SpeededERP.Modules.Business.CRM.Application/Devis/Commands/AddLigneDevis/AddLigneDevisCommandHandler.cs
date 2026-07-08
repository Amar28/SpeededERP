using MediatR;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Business.CRM.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.CRM.Application.Devis.Commands.AddLigneDevis;

public sealed class AddLigneDevisCommandHandler(IDevisRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<AddLigneDevisCommand, Result>
{
    public async Task<Result> Handle(AddLigneDevisCommand request, CancellationToken cancellationToken)
    {
        var devis = await repository.GetByIdAsync(request.DevisId, cancellationToken).ConfigureAwait(false);
        if (devis is null)
        {
            return Result.Failure(Error.NotFound("Devis.NotFound", "Devis introuvable."));
        }

        var prixUnitaire = Money.Of(request.PrixUnitaire, devis.Devise);
        var result = devis.AjouterLigne(request.Description, request.Quantite, prixUnitaire);
        if (result.IsFailure)
        {
            return result;
        }

        repository.Update(devis);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success();
    }
}
