using Microsoft.EntityFrameworkCore;
using SpeededERP.Modules.Core.Workflow.Domain.Aggregates.WorkflowDefinitions;
using SpeededERP.Modules.Core.Workflow.Domain.Repositories;

namespace SpeededERP.Modules.Core.Workflow.Infrastructure.Persistence.Repositories;

public sealed class WorkflowDefinitionRepository(WorkflowDbContext dbContext) : IWorkflowDefinitionRepository
{
    public Task<WorkflowDefinition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.WorkflowDefinitions.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

    public void Add(WorkflowDefinition aggregate) => dbContext.WorkflowDefinitions.Add(aggregate);

    public void Update(WorkflowDefinition aggregate) => dbContext.WorkflowDefinitions.Update(aggregate);

    public void Remove(WorkflowDefinition aggregate) => dbContext.WorkflowDefinitions.Remove(aggregate);
}
