using MediatR;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Business.CRM.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.CRM.Application.Devis.Commands.SendDevis;

public sealed class SendDevisCommandHandler(IDevisRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<SendDevisCommand, Result>
{
    public async Task<Result> Handle(SendDevisCommand request, CancellationToken cancellationToken)
    {
        var devis = await repository.GetByIdAsync(request.DevisId, cancellationToken).ConfigureAwait(false);
        if (devis is null)
        {
            return Result.Failure(Error.NotFound("Devis.NotFound", "Devis introuvable."));
        }

        var result = devis.Envoyer();
        if (result.IsFailure)
        {
            return result;
        }

        repository.Update(devis);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success();
    }
}
