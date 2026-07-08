using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Business.Logistique.Application.Expeditions.Commands.PreparerExpedition;

public sealed record PreparerExpeditionCommand(
    string NumeroSuivi,
    string TransporteurNom,
    string Street,
    string City,
    string PostalCode,
    string CountryIso2) : ICommand<Guid>;
