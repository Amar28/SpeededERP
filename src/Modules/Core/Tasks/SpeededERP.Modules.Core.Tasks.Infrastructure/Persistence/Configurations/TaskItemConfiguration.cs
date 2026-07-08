using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskItemAggregate = SpeededERP.Modules.Core.Tasks.Domain.Aggregates.TaskItems.TaskItem;

namespace SpeededERP.Modules.Core.Tasks.Infrastructure.Persistence.Configurations;

public sealed class TaskItemConfiguration : IEntityTypeConfiguration<TaskItemAggregate>
{
    public void Configure(EntityTypeBuilder<TaskItemAggregate> builder)
    {
        builder.ToTable("task_items");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title).HasMaxLength(256).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(2000);
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(16);
        builder.HasIndex(t => new { t.TenantId, t.AssigneeUserId });

        builder.Ignore(t => t.DomainEvents);
    }
}
