namespace SpeededERP.Modules.Core.Metadata.Application.FormDefinitions.Queries.GetFormDefinition;

public sealed record FormDefinitionDto(Guid Id, Guid EntityDefinitionId, string Name, string LayoutJson, int Version, bool IsPublished);
