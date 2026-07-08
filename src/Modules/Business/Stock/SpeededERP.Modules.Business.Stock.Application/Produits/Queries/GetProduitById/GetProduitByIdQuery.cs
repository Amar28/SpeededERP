using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Business.Stock.Application.Produits.Queries.GetProduitById;

public sealed record GetProduitByIdQuery(Guid ProduitId) : IQuery<ProduitDto>;
