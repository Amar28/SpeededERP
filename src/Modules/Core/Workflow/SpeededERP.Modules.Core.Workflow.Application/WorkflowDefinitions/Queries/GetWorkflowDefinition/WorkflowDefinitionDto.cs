namespace SpeededERP.Modules.Core.Workflow.Application.WorkflowDefinitions.Queries.GetWorkflowDefinition;

public sealed record WorkflowDefinitionDto(Guid Id, string Name, Guid? EntityDefinitionId, string DefinitionJson, bool IsActive);
