using Microsoft.EntityFrameworkCore;
using SpeededERP.BuildingBlocks.Infrastructure.Persistence;
using SpeededERP.Modules.Business.Logistique.Domain.Aggregates.Expeditions;
using SpeededERP.Modules.Business.Logistique.Infrastructure.Persistence.Configurations;

namespace SpeededERP.Modules.Business.Logistique.Infrastructure.Persistence;

public sealed class LogistiqueDbContext(DbContextOptions<LogistiqueDbContext> options) : AppDbContextBase(options)
{
    protected override string ModuleSchema => "logistique";

    public DbSet<Expedition> Expeditions => Set<Expedition>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ExpeditionConfiguration());
    }
}
