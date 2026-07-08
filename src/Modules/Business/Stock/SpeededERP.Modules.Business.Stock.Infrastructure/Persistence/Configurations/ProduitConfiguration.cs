using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeededERP.Modules.Business.Stock.Domain.Aggregates.Produits;

namespace SpeededERP.Modules.Business.Stock.Infrastructure.Persistence.Configurations;

public sealed class ProduitConfiguration : IEntityTypeConfiguration<Produit>
{
    public void Configure(EntityTypeBuilder<Produit> builder)
    {
        builder.ToTable("produits");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Reference).HasMaxLength(64).IsRequired();
        builder.Property(p => p.Nom).HasMaxLength(256).IsRequired();
        builder.Property(p => p.QuantiteEnStock).IsRequired();
        builder.Property(p => p.SeuilAlerte).IsRequired();

        builder.HasIndex(p => new { p.TenantId, p.Reference }).IsUnique();

        builder.OwnsOne(p => p.PrixUnitaire, money =>
        {
            money.Property(m => m.Amount).HasColumnName("prix_unitaire_amount").HasColumnType("numeric(18,4)");
            money.Property(m => m.Currency).HasColumnName("prix_unitaire_devise").HasMaxLength(3);
        });
        builder.Navigation(p => p.PrixUnitaire).IsRequired();

        builder.Ignore(p => p.EstEnAlerte);
        builder.Ignore(p => p.DomainEvents);
    }
}
