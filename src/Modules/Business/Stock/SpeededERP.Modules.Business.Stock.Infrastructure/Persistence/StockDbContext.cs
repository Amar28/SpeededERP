using Microsoft.EntityFrameworkCore;
using SpeededERP.BuildingBlocks.Infrastructure.Persistence;
using SpeededERP.Modules.Business.Stock.Domain.Aggregates.Produits;
using SpeededERP.Modules.Business.Stock.Infrastructure.Persistence.Configurations;

namespace SpeededERP.Modules.Business.Stock.Infrastructure.Persistence;

public sealed class StockDbContext(DbContextOptions<StockDbContext> options) : AppDbContextBase(options)
{
    protected override string ModuleSchema => "stock";

    public DbSet<Produit> Produits => Set<Produit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ProduitConfiguration());
    }
}
