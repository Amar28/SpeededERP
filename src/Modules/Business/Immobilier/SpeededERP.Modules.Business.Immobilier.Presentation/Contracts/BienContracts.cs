using SpeededERP.Modules.Business.Immobilier.Domain.Aggregates.Biens;

namespace SpeededERP.Modules.Business.Immobilier.Presentation.Contracts;

public sealed record EnregistrerBienRequest(
    string Street,
    string City,
    string PostalCode,
    string CountryIso2,
    TypeBien TypeBien,
    decimal LoyerMensuel,
    string Devise = "EUR");
