using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeededERP.Modules.Core.Metadata.Domain.Aggregates.EntityDefinitions;

namespace SpeededERP.Modules.Core.Metadata.Infrastructure.Persistence.Configurations;

public sealed class EntityDefinitionConfiguration : IEntityTypeConfiguration<EntityDefinition>
{
    public void Configure(EntityTypeBuilder<EntityDefinition> builder)
    {
        builder.ToTable("entity_definitions");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ModuleName).HasMaxLength(128).IsRequired();
        builder.Property(e => e.EntityName).HasMaxLength(128).IsRequired();
        builder.HasIndex(e => new { e.ModuleName, e.EntityName }).IsUnique();
        builder.Property(e => e.DisplayNameKey).HasMaxLength(256).IsRequired();

        builder.Ignore(e => e.DomainEvents);
    }
}
