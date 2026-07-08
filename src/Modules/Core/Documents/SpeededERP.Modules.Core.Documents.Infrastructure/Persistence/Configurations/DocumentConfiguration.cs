using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeededERP.Modules.Core.Documents.Domain.Aggregates.Documents;

namespace SpeededERP.Modules.Core.Documents.Infrastructure.Persistence.Configurations;

public sealed class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("documents");
        builder.HasKey(d => d.Id);
        builder.HasQueryFilter(d => !d.IsDeleted);

        builder.Property(d => d.FileName).HasMaxLength(256).IsRequired();
        builder.Property(d => d.StorageKey).HasMaxLength(1024).IsRequired();
        builder.Property(d => d.ContentType).HasMaxLength(128).IsRequired();
        builder.Property(d => d.EntityType).HasMaxLength(128);
        builder.HasIndex(d => new { d.TenantId, d.EntityType, d.EntityId });

        builder.Ignore(d => d.DomainEvents);
    }
}
