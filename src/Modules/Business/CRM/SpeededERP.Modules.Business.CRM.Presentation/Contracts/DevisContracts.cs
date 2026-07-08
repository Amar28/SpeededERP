namespace SpeededERP.Modules.Business.CRM.Presentation.Contracts;

public sealed record CreateDevisRequest(Guid ClientId, string Devise = "EUR");

public sealed record AddLigneDevisRequest(string Description, decimal Quantite, decimal PrixUnitaire);
