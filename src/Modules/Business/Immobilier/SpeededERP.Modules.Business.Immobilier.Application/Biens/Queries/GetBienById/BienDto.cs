namespace SpeededERP.Modules.Business.Immobilier.Application.Biens.Queries.GetBienById;

public sealed record BienDto(
    Guid Id,
    string Street,
    string City,
    string PostalCode,
    string CountryIso2,
    string TypeBien,
    decimal LoyerMensuel,
    string Devise,
    string StatutOccupation);
