using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Business.RH.Domain.Aggregates.Employes;
using SpeededERP.Modules.Business.RH.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.RH.Application.Employes.Commands.EmbaucherEmploye;

public sealed class EmbaucherEmployeCommandHandler(IEmployeRepository repository, [FromKeyedServices("rh")] IUnitOfWork unitOfWork)
    : IRequestHandler<EmbaucherEmployeCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(EmbaucherEmployeCommand request, CancellationToken cancellationToken)
    {
        var employeResult = Employe.Embaucher(request.Nom, request.Prenom, request.Email, request.Poste, request.DateEmbaucheUtc);
        if (employeResult.IsFailure)
        {
            return Result.Failure<Guid>(employeResult.Error);
        }

        repository.Add(employeResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(employeResult.Value.Id);
    }
}
