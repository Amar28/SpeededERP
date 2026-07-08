using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeededERP.Modules.Core.Metadata.Domain.Aggregates.FieldDefinitions;

namespace SpeededERP.Modules.Core.Metadata.Infrastructure.Persistence.Configurations;

public sealed class FieldDefinitionConfiguration : IEntityTypeConfiguration<FieldDefinition>
{
    public void Configure(EntityTypeBuilder<FieldDefinition> builder)
    {
        builder.ToTable("field_definitions");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Key).HasMaxLength(64).IsRequired();
        builder.HasIndex(f => new { f.TenantId, f.EntityDefinitionId, f.Key }).IsUnique();
        builder.Property(f => f.LabelKey).HasMaxLength(256).IsRequired();
        builder.Property(f => f.Type).HasConversion<string>().HasMaxLength(32);
        builder.Property(f => f.ValidationRuleJson).HasColumnType("jsonb");

        builder.PrimitiveCollection<List<string>>("_listOptions")
            .HasField("_listOptions")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("list_options")
            .HasColumnType("text[]");

        builder.Ignore(f => f.ListOptions);
        builder.Ignore(f => f.DomainEvents);
    }
}
