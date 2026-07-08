using MediatR;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Business.Logistique.Domain.Aggregates.Expeditions;
using SpeededERP.Modules.Business.Logistique.Domain.Repositories;
using SpeededERP.Modules.Business.Logistique.Domain.ValueObjects;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.Logistique.Application.Expeditions.Commands.PreparerExpedition;

public sealed class PreparerExpeditionCommandHandler(IExpeditionRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<PreparerExpeditionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(PreparerExpeditionCommand request, CancellationToken cancellationToken)
    {
        var adresseResult = AdresseLivraison.Create(request.Street, request.City, request.PostalCode, request.CountryIso2);
        if (adresseResult.IsFailure)
        {
            return Result.Failure<Guid>(adresseResult.Error);
        }

        var expeditionResult = Expedition.Preparer(request.NumeroSuivi, request.TransporteurNom, adresseResult.Value);
        if (expeditionResult.IsFailure)
        {
            return Result.Failure<Guid>(expeditionResult.Error);
        }

        repository.Add(expeditionResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(expeditionResult.Value.Id);
    }
}
