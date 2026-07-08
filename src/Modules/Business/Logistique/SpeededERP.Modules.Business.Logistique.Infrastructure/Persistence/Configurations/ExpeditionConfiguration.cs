using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeededERP.Modules.Business.Logistique.Domain.Aggregates.Expeditions;

namespace SpeededERP.Modules.Business.Logistique.Infrastructure.Persistence.Configurations;

public sealed class ExpeditionConfiguration : IEntityTypeConfiguration<Expedition>
{
    public void Configure(EntityTypeBuilder<Expedition> builder)
    {
        builder.ToTable("expeditions");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.NumeroSuivi).HasMaxLength(128).IsRequired();
        builder.Property(e => e.TransporteurNom).HasMaxLength(256).IsRequired();
        builder.Property(e => e.Statut).HasConversion<string>().HasMaxLength(16);

        builder.OwnsOne(e => e.AdresseLivraison, address =>
        {
            address.Property(a => a.Street).HasColumnName("adresse_street").HasMaxLength(256).IsRequired();
            address.Property(a => a.City).HasColumnName("adresse_city").HasMaxLength(128).IsRequired();
            address.Property(a => a.PostalCode).HasColumnName("adresse_postal_code").HasMaxLength(16).IsRequired();
            address.Property(a => a.CountryIso2).HasColumnName("adresse_country").HasMaxLength(2).IsRequired();
        });
        builder.Navigation(e => e.AdresseLivraison).IsRequired();

        builder.Ignore(e => e.DomainEvents);
    }
}
