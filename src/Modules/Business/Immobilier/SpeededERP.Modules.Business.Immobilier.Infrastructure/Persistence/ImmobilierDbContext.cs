using Microsoft.EntityFrameworkCore;
using SpeededERP.BuildingBlocks.Infrastructure.Persistence;
using SpeededERP.Modules.Business.Immobilier.Domain.Aggregates.Biens;
using SpeededERP.Modules.Business.Immobilier.Infrastructure.Persistence.Configurations;

namespace SpeededERP.Modules.Business.Immobilier.Infrastructure.Persistence;

public sealed class ImmobilierDbContext(DbContextOptions<ImmobilierDbContext> options) : AppDbContextBase(options)
{
    protected override string ModuleSchema => "immobilier";

    public DbSet<Bien> Biens => Set<Bien>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new BienConfiguration());
    }
}
