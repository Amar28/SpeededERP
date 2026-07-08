using MediatR;
using SpeededERP.Modules.Business.Stock.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.Stock.Application.Produits.Queries.GetProduitById;

public sealed class GetProduitByIdQueryHandler(IProduitRepository repository) : IRequestHandler<GetProduitByIdQuery, Result<ProduitDto>>
{
    public async Task<Result<ProduitDto>> Handle(GetProduitByIdQuery request, CancellationToken cancellationToken)
    {
        var produit = await repository.GetByIdAsync(request.ProduitId, cancellationToken).ConfigureAwait(false);
        if (produit is null)
        {
            return Result.Failure<ProduitDto>(Error.NotFound("Produit.NotFound", "Produit introuvable."));
        }

        var dto = new ProduitDto(
            produit.Id,
            produit.Reference,
            produit.Nom,
            produit.QuantiteEnStock,
            produit.SeuilAlerte,
            produit.PrixUnitaire.Amount,
            produit.PrixUnitaire.Currency,
            produit.EstEnAlerte);

        return Result.Success(dto);
    }
}
