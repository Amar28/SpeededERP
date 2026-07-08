using System.Text.Json;

namespace SpeededERP.Modules.Business.CRM.Presentation.Contracts;

public sealed record RegisterClientRequest(
    string Name,
    string Street,
    string City,
    string PostalCode,
    string CountryIso2,
    string? Phone,
    JsonElement? CustomFields);
