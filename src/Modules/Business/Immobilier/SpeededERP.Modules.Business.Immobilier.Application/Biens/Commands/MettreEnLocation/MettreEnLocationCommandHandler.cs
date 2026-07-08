using MediatR;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Business.Immobilier.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.Immobilier.Application.Biens.Commands.MettreEnLocation;

public sealed class MettreEnLocationCommandHandler(IBienRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<MettreEnLocationCommand, Result>
{
    public async Task<Result> Handle(MettreEnLocationCommand request, CancellationToken cancellationToken)
    {
        var bien = await repository.GetByIdAsync(request.BienId, cancellationToken).ConfigureAwait(false);
        if (bien is null)
        {
            return Result.Failure(Error.NotFound("Bien.NotFound", "Bien introuvable."));
        }

        var result = bien.MettreEnLocation();
        if (result.IsFailure)
        {
            return result;
        }

        repository.Update(bien);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success();
    }
}
