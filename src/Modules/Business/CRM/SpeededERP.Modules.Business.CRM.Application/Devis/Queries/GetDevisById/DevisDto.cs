namespace SpeededERP.Modules.Business.CRM.Application.Devis.Queries.GetDevisById;

public sealed record LigneDevisDto(Guid Id, string Description, decimal Quantite, decimal PrixUnitaire, decimal MontantLigne);

public sealed record DevisDto(
    Guid Id,
    Guid ClientId,
    string Statut,
    string Devise,
    decimal Montant,
    IReadOnlyCollection<LigneDevisDto> Lignes);
