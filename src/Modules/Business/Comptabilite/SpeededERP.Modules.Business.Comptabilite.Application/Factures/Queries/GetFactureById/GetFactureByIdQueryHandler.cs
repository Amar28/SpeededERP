using MediatR;
using SpeededERP.Modules.Business.Comptabilite.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.Comptabilite.Application.Factures.Queries.GetFactureById;

public sealed class GetFactureByIdQueryHandler(IFactureRepository repository) : IRequestHandler<GetFactureByIdQuery, Result<FactureDto>>
{
    public async Task<Result<FactureDto>> Handle(GetFactureByIdQuery request, CancellationToken cancellationToken)
    {
        var facture = await repository.GetByIdAsync(request.FactureId, cancellationToken).ConfigureAwait(false);
        if (facture is null)
        {
            return Result.Failure<FactureDto>(Error.NotFound("Facture.NotFound", "Facture introuvable."));
        }

        var dto = new FactureDto(
            facture.Id,
            facture.ClientId,
            facture.Montant.Amount,
            facture.Montant.Currency,
            facture.Statut.ToString(),
            facture.DateEmissionUtc,
            facture.ReferenceExterne);

        return Result.Success(dto);
    }
}
