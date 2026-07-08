using SpeededERP.Modules.Core.Metadata.Domain.Aggregates.FieldDefinitions;

namespace SpeededERP.Modules.Core.Metadata.Application.FieldDefinitions.Queries.GetFieldDefinitionsByEntity;

public sealed record FieldDefinitionDto(
    Guid Id,
    string Key,
    string LabelKey,
    FieldType Type,
    bool IsRequired,
    int Order,
    IReadOnlyCollection<string> ListOptions);
