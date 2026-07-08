using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Identity.Domain.Events;

public sealed record UserRegisteredDomainEvent(Guid UserId, Guid TenantId, string Email) : DomainEvent;

public sealed record UserActivatedDomainEvent(Guid UserId, Guid TenantId) : DomainEvent;

public sealed record UserDeactivatedDomainEvent(Guid UserId, Guid TenantId) : DomainEvent;

public sealed record RoleAssignedToUserDomainEvent(Guid UserId, Guid RoleId, Guid TenantId) : DomainEvent;
