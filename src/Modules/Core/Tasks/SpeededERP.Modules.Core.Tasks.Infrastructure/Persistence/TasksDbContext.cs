using Microsoft.EntityFrameworkCore;
using SpeededERP.BuildingBlocks.Infrastructure.Persistence;
using SpeededERP.Modules.Core.Tasks.Infrastructure.Persistence.Configurations;
using TaskItemAggregate = SpeededERP.Modules.Core.Tasks.Domain.Aggregates.TaskItems.TaskItem;

namespace SpeededERP.Modules.Core.Tasks.Infrastructure.Persistence;

public sealed class TasksDbContext(DbContextOptions<TasksDbContext> options) : AppDbContextBase(options)
{
    protected override string ModuleSchema => "tasks";

    public DbSet<TaskItemAggregate> TaskItems => Set<TaskItemAggregate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new TaskItemConfiguration());
    }
}
