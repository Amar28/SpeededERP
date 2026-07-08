using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Business.Stock.Application.Produits.Commands.CreerProduit;

public sealed record CreerProduitCommand(string Reference, string Nom, decimal PrixUnitaire, string Devise, int SeuilAlerte) : ICommand<Guid>;
