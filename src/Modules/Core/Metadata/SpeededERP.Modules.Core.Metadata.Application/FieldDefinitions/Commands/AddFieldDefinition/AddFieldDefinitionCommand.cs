using SpeededERP.BuildingBlocks.Application.Messaging;
using SpeededERP.BuildingBlocks.Application.Security;
using SpeededERP.Modules.Core.Metadata.Domain.Aggregates.FieldDefinitions;

namespace SpeededERP.Modules.Core.Metadata.Application.FieldDefinitions.Commands.AddFieldDefinition;

[RequirePermission("metadata:manage-forms")]
public sealed record AddFieldDefinitionCommand(
    Guid EntityDefinitionId,
    string Key,
    string LabelKey,
    FieldType Type,
    bool IsRequired,
    int Order,
    IReadOnlyCollection<string>? ListOptions,
    string? ValidationRuleJson) : ICommand<Guid>;
