using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.CRM.Domain.Events;

public sealed record DevisEnvoyeDomainEvent(Guid DevisId, Guid TenantId) : DomainEvent;

/// <summary>
/// The event walked through end-to-end in architecture section 09, Fig. 03: raised by
/// <c>Devis.Accepter()</c>, dispatched in-memory, and separately translated by the
/// Application handler into <c>DevisAcceptedIntegrationEvent</c> for the Outbox.
/// </summary>
public sealed record DevisAcceptedDomainEvent(Guid DevisId, Guid ClientId, Guid TenantId, decimal Montant, string Devise) : DomainEvent;
