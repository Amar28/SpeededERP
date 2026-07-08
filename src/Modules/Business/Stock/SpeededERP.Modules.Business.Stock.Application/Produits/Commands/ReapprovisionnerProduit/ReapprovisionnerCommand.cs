using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Business.Stock.Application.Produits.Commands.ReapprovisionnerProduit;

public sealed record ReapprovisionnerCommand(Guid ProduitId, int Quantite) : ICommand;
