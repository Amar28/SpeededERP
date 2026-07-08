using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeededERP.Modules.Core.Identity.Domain.Aggregates.Roles;

namespace SpeededERP.Modules.Core.Identity.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("business_roles");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name).HasMaxLength(128).IsRequired();
        builder.HasIndex(r => new { r.TenantId, r.Name }).IsUnique();
        builder.Property(r => r.Description).HasMaxLength(512);
        builder.Property(r => r.IsSystemRole);

        builder.PrimitiveCollection<List<string>>("_permissions")
            .HasField("_permissions")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("permissions")
            .HasColumnType("text[]");

        builder.Ignore(r => r.Permissions);
        builder.Ignore(r => r.DomainEvents);
    }
}
