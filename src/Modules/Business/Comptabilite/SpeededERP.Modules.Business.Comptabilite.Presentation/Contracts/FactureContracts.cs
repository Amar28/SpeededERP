namespace SpeededERP.Modules.Business.Comptabilite.Presentation.Contracts;

public sealed record CreerFactureRequest(Guid ClientId, decimal Montant, string Devise, string? ReferenceExterne);
