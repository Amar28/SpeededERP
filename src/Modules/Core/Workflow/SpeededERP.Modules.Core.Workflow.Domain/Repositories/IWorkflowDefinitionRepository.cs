using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Workflow.Domain.Aggregates.WorkflowDefinitions;

namespace SpeededERP.Modules.Core.Workflow.Domain.Repositories;

public interface IWorkflowDefinitionRepository : IRepository<WorkflowDefinition, Guid>
{
}
