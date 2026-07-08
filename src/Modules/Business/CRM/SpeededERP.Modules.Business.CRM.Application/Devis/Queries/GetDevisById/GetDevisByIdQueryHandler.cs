using MediatR;
using SpeededERP.Modules.Business.CRM.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.CRM.Application.Devis.Queries.GetDevisById;

public sealed class GetDevisByIdQueryHandler(IDevisRepository repository) : IRequestHandler<GetDevisByIdQuery, Result<DevisDto>>
{
    public async Task<Result<DevisDto>> Handle(GetDevisByIdQuery request, CancellationToken cancellationToken)
    {
        var devis = await repository.GetByIdAsync(request.DevisId, cancellationToken).ConfigureAwait(false);
        if (devis is null)
        {
            return Result.Failure<DevisDto>(Error.NotFound("Devis.NotFound", "Devis introuvable."));
        }

        var lignes = devis.Lignes
            .Select(l => new LigneDevisDto(l.Id, l.Description, l.Quantite, l.PrixUnitaire.Amount, l.MontantLigne.Amount))
            .ToList();

        var dto = new DevisDto(devis.Id, devis.ClientId, devis.Statut.ToString(), devis.Devise, devis.Montant.Amount, lignes);

        return Result.Success(dto);
    }
}
