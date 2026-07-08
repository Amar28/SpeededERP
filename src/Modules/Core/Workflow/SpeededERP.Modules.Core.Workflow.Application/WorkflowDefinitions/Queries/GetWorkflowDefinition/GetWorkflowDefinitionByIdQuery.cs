using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Core.Workflow.Application.WorkflowDefinitions.Queries.GetWorkflowDefinition;

public sealed record GetWorkflowDefinitionByIdQuery(Guid WorkflowDefinitionId) : IQuery<WorkflowDefinitionDto>, ICacheableQuery
{
    public string CacheKey => $"workflow:definition:{WorkflowDefinitionId}";

    public TimeSpan? Expiration => TimeSpan.FromMinutes(30);
}
