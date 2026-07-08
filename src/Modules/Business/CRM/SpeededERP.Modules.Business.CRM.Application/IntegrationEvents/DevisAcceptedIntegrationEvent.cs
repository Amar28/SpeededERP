using SpeededERP.BuildingBlocks.Messaging;

namespace SpeededERP.Modules.Business.CRM.Application.IntegrationEvents;

/// <summary>
/// Architecture section 09, Fig. 03: consumed by Comptabilité (creates the invoice),
/// Notifications (confirms to the client) and Search/Audit — CRM never knows who
/// listens, only this contract.
/// </summary>
public sealed record DevisAcceptedIntegrationEvent : IntegrationEvent
{
    public required Guid DevisId { get; init; }

    public required Guid ClientId { get; init; }

    public required decimal Montant { get; init; }

    public required string Devise { get; init; }
}
