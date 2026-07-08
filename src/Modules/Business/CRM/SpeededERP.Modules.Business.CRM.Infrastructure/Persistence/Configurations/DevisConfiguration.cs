using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeededERP.Modules.Business.CRM.Domain.Aggregates.Devis;

namespace SpeededERP.Modules.Business.CRM.Infrastructure.Persistence.Configurations;

public sealed class DevisConfiguration : IEntityTypeConfiguration<Devis>
{
    public void Configure(EntityTypeBuilder<Devis> builder)
    {
        builder.ToTable("devis");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.ClientId).IsRequired();
        builder.Property(d => d.Statut).HasConversion<string>().HasMaxLength(16);
        builder.Property(d => d.Devise).HasMaxLength(3).IsRequired();

        // LigneDevis is owned by Devis — never persisted or queried independently
        // (consistency boundary, architecture section 07).
        builder.OwnsMany(d => d.Lignes, lignes =>
        {
            lignes.ToTable("devis_lignes");
            lignes.WithOwner().HasForeignKey("DevisId");
            lignes.HasKey(l => l.Id);
            lignes.UsePropertyAccessMode(PropertyAccessMode.Field);

            lignes.Property(l => l.Description).HasMaxLength(512).IsRequired();
            lignes.Property(l => l.Quantite).HasColumnType("numeric(18,4)");

            lignes.OwnsOne(l => l.PrixUnitaire, money =>
            {
                money.Property(m => m.Amount).HasColumnName("prix_unitaire_amount").HasColumnType("numeric(18,4)");
                money.Property(m => m.Currency).HasColumnName("prix_unitaire_devise").HasMaxLength(3);
            });
        });

        builder.Navigation(d => d.Lignes).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(d => d.Montant);
        builder.Ignore(d => d.DomainEvents);
    }
}
