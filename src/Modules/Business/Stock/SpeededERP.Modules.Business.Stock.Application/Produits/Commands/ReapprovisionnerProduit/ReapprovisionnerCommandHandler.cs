using MediatR;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Business.Stock.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.Stock.Application.Produits.Commands.ReapprovisionnerProduit;

public sealed class ReapprovisionnerCommandHandler(IProduitRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<ReapprovisionnerCommand, Result>
{
    public async Task<Result> Handle(ReapprovisionnerCommand request, CancellationToken cancellationToken)
    {
        var produit = await repository.GetByIdAsync(request.ProduitId, cancellationToken).ConfigureAwait(false);
        if (produit is null)
        {
            return Result.Failure(Error.NotFound("Produit.NotFound", "Produit introuvable."));
        }

        var result = produit.Reapprovisionner(request.Quantite);
        if (result.IsFailure)
        {
            return result;
        }

        repository.Update(produit);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success();
    }
}
