using SpeededERP.BuildingBlocks.Messaging;

namespace SpeededERP.Modules.Core.Identity.Application.IntegrationEvents;

/// <summary>
/// Published after a user is registered so other modules (Notifications, Collaboration)
/// can react without ever referencing Core.Identity's internals (section 09).
/// </summary>
public sealed record UserRegisteredIntegrationEvent : IntegrationEvent
{
    public required Guid UserId { get; init; }

    public required string Email { get; init; }

    public required string FullName { get; init; }
}
