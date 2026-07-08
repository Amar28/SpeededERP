namespace SpeededERP.Modules.Business.Comptabilite.Application.Factures.Queries.GetFactureById;

public sealed record FactureDto(
    Guid Id,
    Guid ClientId,
    decimal Montant,
    string Devise,
    string Statut,
    DateTimeOffset? DateEmissionUtc,
    string? ReferenceExterne);
