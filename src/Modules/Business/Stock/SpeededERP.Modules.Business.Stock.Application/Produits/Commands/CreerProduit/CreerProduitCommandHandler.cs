using MediatR;
using SpeededERP.BuildingBlocks.Application.Security;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Business.Stock.Domain.Aggregates.Produits;
using SpeededERP.Modules.Business.Stock.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.Stock.Application.Produits.Commands.CreerProduit;

public sealed class CreerProduitCommandHandler(IProduitRepository repository, IUnitOfWork unitOfWork, ITenantContext tenantContext)
    : IRequestHandler<CreerProduitCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreerProduitCommand request, CancellationToken cancellationToken)
    {
        var referenceExiste = await repository.ReferenceExisteAsync(request.Reference, tenantContext.TenantId, cancellationToken).ConfigureAwait(false);
        if (referenceExiste)
        {
            return Result.Failure<Guid>(Error.Conflict("Produit.ReferenceExistante", "Un produit avec cette référence existe déjà."));
        }

        var prixUnitaire = Money.Of(request.PrixUnitaire, request.Devise);
        var produitResult = Produit.Creer(request.Reference, request.Nom, prixUnitaire, request.SeuilAlerte);
        if (produitResult.IsFailure)
        {
            return Result.Failure<Guid>(produitResult.Error);
        }

        repository.Add(produitResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(produitResult.Value.Id);
    }
}
