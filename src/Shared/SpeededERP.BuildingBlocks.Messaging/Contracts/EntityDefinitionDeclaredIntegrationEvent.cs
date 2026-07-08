namespace SpeededERP.BuildingBlocks.Messaging.Contracts;

/// <summary>
/// Published by every module at startup for each entity it owns and wants extensible
/// via the no-code engine (architecture section 10). Consumed exclusively by
/// Core.Metadata. Living in BuildingBlocks.Messaging (rather than inside
/// Core.Metadata.Application) is the one deliberate exception to "no module references
/// another module" — this is a platform-wide contract, not Core.Metadata's internal
/// model, so every producing module may depend on it without depending on Core.Metadata.
/// </summary>
public sealed record EntityDefinitionDeclaredIntegrationEvent : IntegrationEvent
{
    public required string ModuleName { get; init; }

    public required string EntityName { get; init; }

    public required string DisplayNameKey { get; init; }

    public bool SupportsCustomFields { get; init; } = true;
}
