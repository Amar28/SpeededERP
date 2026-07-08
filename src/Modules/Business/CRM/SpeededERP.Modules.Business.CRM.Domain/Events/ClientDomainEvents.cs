using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.CRM.Domain.Events;

public sealed record ClientRegisteredDomainEvent(Guid ClientId, Guid TenantId) : DomainEvent;
