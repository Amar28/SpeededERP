using Microsoft.EntityFrameworkCore;
using SpeededERP.BuildingBlocks.Infrastructure.Persistence;
using SpeededERP.Modules.Core.Workflow.Domain.Aggregates.WorkflowDefinitions;
using SpeededERP.Modules.Core.Workflow.Infrastructure.Persistence.Configurations;

namespace SpeededERP.Modules.Core.Workflow.Infrastructure.Persistence;

public sealed class WorkflowDbContext(DbContextOptions<WorkflowDbContext> options) : AppDbContextBase(options)
{
    protected override string ModuleSchema => "workflow";

    public DbSet<WorkflowDefinition> WorkflowDefinitions => Set<WorkflowDefinition>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new WorkflowDefinitionConfiguration());
    }
}
