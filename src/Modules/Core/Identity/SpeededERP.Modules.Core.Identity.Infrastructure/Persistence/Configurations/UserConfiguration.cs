using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeededERP.Modules.Core.Identity.Domain.Aggregates.Users;
using SpeededERP.Modules.Core.Identity.Domain.ValueObjects;

namespace SpeededERP.Modules.Core.Identity.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("business_users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .HasConversion(email => email.Value, value => EmailAddress.Create(value).Value)
            .HasMaxLength(320)
            .IsRequired();
        builder.HasIndex(u => new { u.TenantId, u.Email }).IsUnique();

        builder.Property(u => u.FirstName).HasMaxLength(128).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(128).IsRequired();
        builder.Property(u => u.Status).HasConversion<string>().HasMaxLength(32);

        // EF Core 8 primitive collection, mapped straight to a Postgres uuid[] column —
        // simpler and more portable than a join table for a value-only role assignment list.
        builder.PrimitiveCollection<List<Guid>>("_roleIds")
            .HasField("_roleIds")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("role_ids")
            .HasColumnType("uuid[]");

        builder.Ignore(u => u.RoleIds);
        builder.Ignore(u => u.DomainEvents);
    }
}
