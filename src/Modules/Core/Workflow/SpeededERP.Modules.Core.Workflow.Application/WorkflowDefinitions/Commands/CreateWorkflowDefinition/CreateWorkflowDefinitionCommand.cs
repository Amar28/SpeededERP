using SpeededERP.BuildingBlocks.Application.Messaging;
using SpeededERP.BuildingBlocks.Application.Security;

namespace SpeededERP.Modules.Core.Workflow.Application.WorkflowDefinitions.Commands.CreateWorkflowDefinition;

/// <summary>Creates a new WorkflowDefinition, inactive by default.</summary>
[RequirePermission("workflow:manage-definitions")]
public sealed record CreateWorkflowDefinitionCommand(
    string Name,
    Guid? EntityDefinitionId,
    string DefinitionJson) : ICommand<Guid>;
