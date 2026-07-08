namespace SpeededERP.Modules.Core.Workflow.Presentation.Contracts;

public sealed record CreateWorkflowDefinitionRequest(string Name, Guid? EntityDefinitionId, string DefinitionJson);
