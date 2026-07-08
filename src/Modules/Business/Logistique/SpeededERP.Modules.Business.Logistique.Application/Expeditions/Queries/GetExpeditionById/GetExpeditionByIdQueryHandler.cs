using MediatR;
using SpeededERP.Modules.Business.Logistique.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.Logistique.Application.Expeditions.Queries.GetExpeditionById;

public sealed class GetExpeditionByIdQueryHandler(IExpeditionRepository repository) : IRequestHandler<GetExpeditionByIdQuery, Result<ExpeditionDto>>
{
    public async Task<Result<ExpeditionDto>> Handle(GetExpeditionByIdQuery request, CancellationToken cancellationToken)
    {
        var expedition = await repository.GetByIdAsync(request.ExpeditionId, cancellationToken).ConfigureAwait(false);
        if (expedition is null)
        {
            return Result.Failure<ExpeditionDto>(Error.NotFound("Expedition.NotFound", "Expédition introuvable."));
        }

        var dto = new ExpeditionDto(expedition.Id, expedition.NumeroSuivi, expedition.TransporteurNom, expedition.Statut.ToString());

        return Result.Success(dto);
    }
}
