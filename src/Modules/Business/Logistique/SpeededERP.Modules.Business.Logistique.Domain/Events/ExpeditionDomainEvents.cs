using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.Logistique.Domain.Events;

/// <summary>Raised by <c>Expedition.ConfirmerLivraison()</c> — see architecture section 09 for the event-flow pattern.</summary>
public sealed record ExpeditionLivreeDomainEvent(Guid ExpeditionId, Guid TenantId) : DomainEvent;
