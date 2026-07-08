using MediatR;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Business.CRM.Domain.Repositories;
using SpeededERP.SharedKernel;
using DevisAggregate = SpeededERP.Modules.Business.CRM.Domain.Aggregates.Devis.Devis;

namespace SpeededERP.Modules.Business.CRM.Application.Devis.Commands.CreateDevis;

public sealed class CreateDevisCommandHandler(IDevisRepository repository, IClientRepository clientRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateDevisCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateDevisCommand request, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetByIdAsync(request.ClientId, cancellationToken).ConfigureAwait(false);
        if (client is null)
        {
            return Result.Failure<Guid>(Error.NotFound("Client.NotFound", "Client introuvable."));
        }

        var devisResult = DevisAggregate.Creer(request.ClientId, request.Devise);
        if (devisResult.IsFailure)
        {
            return Result.Failure<Guid>(devisResult.Error);
        }

        repository.Add(devisResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(devisResult.Value.Id);
    }
}
