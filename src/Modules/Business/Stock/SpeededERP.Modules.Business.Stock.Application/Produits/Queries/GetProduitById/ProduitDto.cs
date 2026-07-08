namespace SpeededERP.Modules.Business.Stock.Application.Produits.Queries.GetProduitById;

public sealed record ProduitDto(
    Guid Id,
    string Reference,
    string Nom,
    int QuantiteEnStock,
    int SeuilAlerte,
    decimal PrixUnitaire,
    string Devise,
    bool EstEnAlerte);
