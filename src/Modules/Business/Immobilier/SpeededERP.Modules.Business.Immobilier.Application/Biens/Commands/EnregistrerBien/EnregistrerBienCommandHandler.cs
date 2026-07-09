using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Business.Immobilier.Domain.Aggregates.Biens;
using SpeededERP.Modules.Business.Immobilier.Domain.Repositories;
using SpeededERP.Modules.Business.Immobilier.Domain.ValueObjects;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.Immobilier.Application.Biens.Commands.EnregistrerBien;

public sealed class EnregistrerBienCommandHandler(IBienRepository repository, [FromKeyedServices("immobilier")] IUnitOfWork unitOfWork)
    : IRequestHandler<EnregistrerBienCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(EnregistrerBienCommand request, CancellationToken cancellationToken)
    {
        var adresseResult = AdresseBien.Create(request.Street, request.City, request.PostalCode, request.CountryIso2);
        if (adresseResult.IsFailure)
        {
            return Result.Failure<Guid>(adresseResult.Error);
        }

        var loyerMensuel = Money.Of(request.LoyerMensuel, request.Devise);
        var bienResult = Bien.Enregistrer(adresseResult.Value, request.TypeBien, loyerMensuel);
        if (bienResult.IsFailure)
        {
            return Result.Failure<Guid>(bienResult.Error);
        }

        repository.Add(bienResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(bienResult.Value.Id);
    }
}
