using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeededERP.Modules.Business.Immobilier.Domain.Aggregates.Biens;

namespace SpeededERP.Modules.Business.Immobilier.Infrastructure.Persistence.Configurations;

public sealed class BienConfiguration : IEntityTypeConfiguration<Bien>
{
    public void Configure(EntityTypeBuilder<Bien> builder)
    {
        builder.ToTable("biens");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.TypeBien).HasConversion<string>().HasMaxLength(16);
        builder.Property(b => b.StatutOccupation).HasConversion<string>().HasMaxLength(16);

        builder.OwnsOne(b => b.Adresse, address =>
        {
            address.Property(a => a.Street).HasColumnName("adresse_street").HasMaxLength(256).IsRequired();
            address.Property(a => a.City).HasColumnName("adresse_city").HasMaxLength(128).IsRequired();
            address.Property(a => a.PostalCode).HasColumnName("adresse_postal_code").HasMaxLength(16).IsRequired();
            address.Property(a => a.CountryIso2).HasColumnName("adresse_country").HasMaxLength(2).IsRequired();
        });
        builder.Navigation(b => b.Adresse).IsRequired();

        builder.OwnsOne(b => b.LoyerMensuel, money =>
        {
            money.Property(m => m.Amount).HasColumnName("loyer_mensuel_amount").HasColumnType("numeric(18,4)");
            money.Property(m => m.Currency).HasColumnName("loyer_mensuel_devise").HasMaxLength(3);
        });
        builder.Navigation(b => b.LoyerMensuel).IsRequired();

        builder.Ignore(b => b.DomainEvents);
    }
}
