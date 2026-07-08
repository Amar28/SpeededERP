namespace SpeededERP.Modules.Business.Logistique.Presentation.Contracts;

public sealed record PreparerExpeditionRequest(
    string NumeroSuivi,
    string TransporteurNom,
    string Street,
    string City,
    string PostalCode,
    string CountryIso2);
