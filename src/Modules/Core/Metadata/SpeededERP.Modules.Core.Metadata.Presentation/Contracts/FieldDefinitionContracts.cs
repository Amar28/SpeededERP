using SpeededERP.Modules.Core.Metadata.Domain.Aggregates.FieldDefinitions;

namespace SpeededERP.Modules.Core.Metadata.Presentation.Contracts;

public sealed record AddFieldDefinitionRequest(
    string Key,
    string LabelKey,
    FieldType Type,
    bool IsRequired,
    int Order,
    IReadOnlyCollection<string>? ListOptions,
    string? ValidationRuleJson);

public sealed record SaveFormDefinitionRequest(Guid? FormDefinitionId, Guid EntityDefinitionId, string Name, string LayoutJson);
