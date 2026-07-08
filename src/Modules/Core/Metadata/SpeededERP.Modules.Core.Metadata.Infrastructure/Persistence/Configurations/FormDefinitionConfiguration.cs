using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeededERP.Modules.Core.Metadata.Domain.Aggregates.FormDefinitions;

namespace SpeededERP.Modules.Core.Metadata.Infrastructure.Persistence.Configurations;

public sealed class FormDefinitionConfiguration : IEntityTypeConfiguration<FormDefinition>
{
    public void Configure(EntityTypeBuilder<FormDefinition> builder)
    {
        builder.ToTable("form_definitions");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Name).HasMaxLength(256).IsRequired();
        builder.Property(f => f.LayoutJson).HasColumnType("jsonb").IsRequired();
        builder.HasIndex(f => new { f.TenantId, f.EntityDefinitionId });

        builder.Ignore(f => f.DomainEvents);
    }
}
