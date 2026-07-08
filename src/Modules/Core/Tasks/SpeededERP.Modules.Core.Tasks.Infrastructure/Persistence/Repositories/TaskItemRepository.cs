using Microsoft.EntityFrameworkCore;
using SpeededERP.Modules.Core.Tasks.Domain.Repositories;
using TaskItemAggregate = SpeededERP.Modules.Core.Tasks.Domain.Aggregates.TaskItems.TaskItem;

namespace SpeededERP.Modules.Core.Tasks.Infrastructure.Persistence.Repositories;

public sealed class TaskItemRepository(TasksDbContext dbContext) : ITaskItemRepository
{
    public Task<TaskItemAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.TaskItems.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public void Add(TaskItemAggregate aggregate) => dbContext.TaskItems.Add(aggregate);

    public void Update(TaskItemAggregate aggregate) => dbContext.TaskItems.Update(aggregate);

    public void Remove(TaskItemAggregate aggregate) => dbContext.TaskItems.Remove(aggregate);
}
