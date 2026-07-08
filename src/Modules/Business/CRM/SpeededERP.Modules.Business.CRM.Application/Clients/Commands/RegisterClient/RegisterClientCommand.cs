using System.Text.Json;
using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Business.CRM.Application.Clients.Commands.RegisterClient;

/// <summary>
/// Reference example for architecture section 10's exact scenario: <paramref name="CustomFields"/>
/// carries whatever the tenant has configured (e.g. "Numéro fiscal", "Secteur" for one
/// tenant, nothing for another) — the command and handler never change per tenant.
/// </summary>
public sealed record RegisterClientCommand(
    string Name,
    string Street,
    string City,
    string PostalCode,
    string CountryIso2,
    string? Phone,
    JsonElement CustomFields) : ICommand<Guid>;
