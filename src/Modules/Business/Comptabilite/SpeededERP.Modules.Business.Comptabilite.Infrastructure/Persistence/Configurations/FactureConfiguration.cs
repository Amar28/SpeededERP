using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeededERP.Modules.Business.Comptabilite.Domain.Aggregates.Factures;

namespace SpeededERP.Modules.Business.Comptabilite.Infrastructure.Persistence.Configurations;

public sealed class FactureConfiguration : IEntityTypeConfiguration<Facture>
{
    public void Configure(EntityTypeBuilder<Facture> builder)
    {
        builder.ToTable("factures");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.ClientId).IsRequired();
        builder.Property(f => f.Statut).HasConversion<string>().HasMaxLength(16);
        builder.Property(f => f.ReferenceExterne).HasMaxLength(128);

        builder.OwnsOne(f => f.Montant, m =>
        {
            m.Property(x => x.Amount).HasColumnName("montant_amount").HasColumnType("numeric(18,4)");
            m.Property(x => x.Currency).HasColumnName("montant_devise").HasMaxLength(3);
        });

        builder.Navigation(f => f.Montant).IsRequired();

        builder.Ignore(f => f.DomainEvents);
    }
}
