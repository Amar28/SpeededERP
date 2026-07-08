namespace SpeededERP.Modules.Business.Stock.Presentation.Contracts;

public sealed record CreerProduitRequest(string Reference, string Nom, decimal PrixUnitaire, string Devise, int SeuilAlerte);

public sealed record ReapprovisionnerRequest(int Quantite);
