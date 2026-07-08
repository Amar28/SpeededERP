using Microsoft.EntityFrameworkCore;
using SpeededERP.BuildingBlocks.Infrastructure.Persistence;
using SpeededERP.Modules.Business.RH.Domain.Aggregates.Employes;
using SpeededERP.Modules.Business.RH.Infrastructure.Persistence.Configurations;

namespace SpeededERP.Modules.Business.RH.Infrastructure.Persistence;

public sealed class RhDbContext(DbContextOptions<RhDbContext> options) : AppDbContextBase(options)
{
    protected override string ModuleSchema => "rh";

    public DbSet<Employe> Employes => Set<Employe>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new EmployeConfiguration());
    }
}
