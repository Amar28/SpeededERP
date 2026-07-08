using Microsoft.EntityFrameworkCore;
using SpeededERP.BuildingBlocks.Infrastructure.Persistence;
using SpeededERP.Modules.Business.Comptabilite.Domain.Aggregates.Factures;
using SpeededERP.Modules.Business.Comptabilite.Infrastructure.Persistence.Configurations;

namespace SpeededERP.Modules.Business.Comptabilite.Infrastructure.Persistence;

public sealed class ComptabiliteDbContext(DbContextOptions<ComptabiliteDbContext> options) : AppDbContextBase(options)
{
    protected override string ModuleSchema => "comptabilite";

    public DbSet<Facture> Factures => Set<Facture>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new FactureConfiguration());
    }
}
