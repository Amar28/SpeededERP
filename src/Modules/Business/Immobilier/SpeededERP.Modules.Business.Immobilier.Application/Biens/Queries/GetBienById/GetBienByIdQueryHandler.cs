using MediatR;
using SpeededERP.Modules.Business.Immobilier.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.Immobilier.Application.Biens.Queries.GetBienById;

public sealed class GetBienByIdQueryHandler(IBienRepository repository) : IRequestHandler<GetBienByIdQuery, Result<BienDto>>
{
    public async Task<Result<BienDto>> Handle(GetBienByIdQuery request, CancellationToken cancellationToken)
    {
        var bien = await repository.GetByIdAsync(request.BienId, cancellationToken).ConfigureAwait(false);
        if (bien is null)
        {
            return Result.Failure<BienDto>(Error.NotFound("Bien.NotFound", "Bien introuvable."));
        }

        var dto = new BienDto(
            bien.Id,
            bien.Adresse.Street,
            bien.Adresse.City,
            bien.Adresse.PostalCode,
            bien.Adresse.CountryIso2,
            bien.TypeBien.ToString(),
            bien.LoyerMensuel.Amount,
            bien.LoyerMensuel.Currency,
            bien.StatutOccupation.ToString());

        return Result.Success(dto);
    }
}
