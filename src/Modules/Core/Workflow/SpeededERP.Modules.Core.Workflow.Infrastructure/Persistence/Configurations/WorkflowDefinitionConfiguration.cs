using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeededERP.Modules.Core.Workflow.Domain.Aggregates.WorkflowDefinitions;

namespace SpeededERP.Modules.Core.Workflow.Infrastructure.Persistence.Configurations;

public sealed class WorkflowDefinitionConfiguration : IEntityTypeConfiguration<WorkflowDefinition>
{
    public void Configure(EntityTypeBuilder<WorkflowDefinition> builder)
    {
        builder.ToTable("workflow_definitions");
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name).HasMaxLength(256).IsRequired();
        builder.Property(w => w.DefinitionJson).HasColumnType("jsonb").IsRequired();
        builder.HasIndex(w => new { w.TenantId, w.EntityDefinitionId });

        builder.Ignore(w => w.DomainEvents);
    }
}
