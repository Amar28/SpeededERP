using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeededERP.Modules.Core.Identity.Domain.Aggregates.Tenants;

namespace SpeededERP.Modules.Core.Identity.Infrastructure.Persistence.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.CompanyName).HasMaxLength(256).IsRequired();
        builder.Property(t => t.Slug).HasMaxLength(63).IsRequired();
        builder.HasIndex(t => t.Slug).IsUnique();
        builder.Property(t => t.IsolationMode).HasConversion<string>().HasMaxLength(32);
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(32);
        builder.Property(t => t.DefaultLanguage).HasMaxLength(8);
        builder.Property(t => t.DefaultCurrency).HasMaxLength(3);

        builder.PrimitiveCollection<List<string>>("_activatedModules")
            .HasField("_activatedModules")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("activated_modules")
            .HasColumnType("text[]");

        builder.Ignore(t => t.ActivatedModules);
        builder.Ignore(t => t.DomainEvents);
    }
}
