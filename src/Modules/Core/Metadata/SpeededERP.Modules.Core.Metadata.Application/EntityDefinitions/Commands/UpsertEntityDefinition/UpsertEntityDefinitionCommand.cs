using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Core.Metadata.Application.EntityDefinitions.Commands.UpsertEntityDefinition;

/// <summary>
/// Idempotent registration of an entity a module owns (architecture section 10).
/// Invoked at module startup — see <c>EntityDefinitionDeclaredConsumer</c> in
/// Infrastructure — never by a tenant.
/// </summary>
public sealed record UpsertEntityDefinitionCommand(
    string ModuleName,
    string EntityName,
    string DisplayNameKey,
    bool SupportsCustomFields) : ICommand<Guid>;
