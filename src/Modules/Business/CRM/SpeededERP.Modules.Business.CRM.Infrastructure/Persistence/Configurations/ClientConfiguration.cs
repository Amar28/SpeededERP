using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeededERP.Modules.Business.CRM.Domain.Aggregates.Clients;
using SpeededERP.Modules.Business.CRM.Domain.ValueObjects;

namespace SpeededERP.Modules.Business.CRM.Infrastructure.Persistence.Configurations;

public sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("clients");
        builder.HasKey(c => c.Id);
        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.Property(c => c.Name).HasMaxLength(256).IsRequired();
        builder.Property(c => c.Phone).HasMaxLength(32);
        builder.Property(c => c.Kind).HasConversion<string>().HasMaxLength(16);

        // Native, code-owned columns (architecture section 10).
        builder.OwnsOne(c => c.Address, address =>
        {
            address.Property(a => a.Street).HasColumnName("address_street").HasMaxLength(256).IsRequired();
            address.Property(a => a.City).HasColumnName("address_city").HasMaxLength(128).IsRequired();
            address.Property(a => a.PostalCode).HasColumnName("address_postal_code").HasMaxLength(16).IsRequired();
            address.Property(a => a.CountryIso2).HasColumnName("address_country").HasMaxLength(2).IsRequired();
        });
        builder.Navigation(c => c.Address).IsRequired();

        // The tenant-extensible bag — same table, sits right next to the native columns above.
        builder.Property(c => c.CustomFieldsJson).HasColumnName("custom_fields").HasColumnType("jsonb").IsRequired();
        builder.HasIndex(c => c.CustomFieldsJson).HasMethod("gin");

        builder.Ignore(c => c.DomainEvents);
    }
}
