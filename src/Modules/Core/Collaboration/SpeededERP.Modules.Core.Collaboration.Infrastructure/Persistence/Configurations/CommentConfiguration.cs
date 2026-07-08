using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeededERP.Modules.Core.Collaboration.Domain.Aggregates.Comments;

namespace SpeededERP.Modules.Core.Collaboration.Infrastructure.Persistence.Configurations;

public sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("comments");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.EntityType).HasMaxLength(128).IsRequired();
        builder.Property(c => c.Body).HasMaxLength(4000).IsRequired();
        builder.HasIndex(c => new { c.TenantId, c.EntityType, c.EntityId });

        builder.Ignore(c => c.DomainEvents);
    }
}
