using MediatR;
using SpeededERP.Modules.Business.RH.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.RH.Application.Employes.Queries.GetEmployeById;

public sealed class GetEmployeByIdQueryHandler(IEmployeRepository repository) : IRequestHandler<GetEmployeByIdQuery, Result<EmployeDto>>
{
    public async Task<Result<EmployeDto>> Handle(GetEmployeByIdQuery request, CancellationToken cancellationToken)
    {
        var employe = await repository.GetByIdAsync(request.EmployeId, cancellationToken).ConfigureAwait(false);
        if (employe is null)
        {
            return Result.Failure<EmployeDto>(Error.NotFound("Employe.NotFound", "Employé introuvable."));
        }

        var dto = new EmployeDto(
            employe.Id,
            employe.Nom,
            employe.Prenom,
            employe.Email,
            employe.Poste,
            employe.DateEmbaucheUtc,
            employe.Statut.ToString());

        return Result.Success(dto);
    }
}
