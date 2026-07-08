using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeededERP.Modules.Business.RH.Domain.Aggregates.Employes;

namespace SpeededERP.Modules.Business.RH.Infrastructure.Persistence.Configurations;

public sealed class EmployeConfiguration : IEntityTypeConfiguration<Employe>
{
    public void Configure(EntityTypeBuilder<Employe> builder)
    {
        builder.ToTable("employes");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nom).HasMaxLength(128).IsRequired();
        builder.Property(e => e.Prenom).HasMaxLength(128).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(256).IsRequired();
        builder.Property(e => e.Poste).HasMaxLength(128).IsRequired();
        builder.Property(e => e.Statut).HasConversion<string>().HasMaxLength(16);

        builder.Ignore(e => e.DomainEvents);
    }
}
