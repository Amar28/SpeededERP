using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TenantSettingsAggregate = SpeededERP.Modules.Core.Config.Domain.Aggregates.TenantSettings.TenantSettings;

namespace SpeededERP.Modules.Core.Config.Infrastructure.Persistence.Configurations;

public sealed class TenantSettingsConfiguration : IEntityTypeConfiguration<TenantSettingsAggregate>
{
    public void Configure(EntityTypeBuilder<TenantSettingsAggregate> builder)
    {
        builder.ToTable("tenant_settings");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.DefaultLanguage).HasMaxLength(8).IsRequired();
        builder.Property(t => t.DefaultCurrency).HasMaxLength(3).IsRequired();
        builder.Property(t => t.TimeZoneId).HasMaxLength(64).IsRequired();

        builder.PrimitiveCollection<List<string>>("_activatedModules")
            .HasField("_activatedModules")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("activated_modules")
            .HasColumnType("text[]");

        builder.Ignore(t => t.ActivatedModules);
        builder.Ignore(t => t.DomainEvents);
    }
}
