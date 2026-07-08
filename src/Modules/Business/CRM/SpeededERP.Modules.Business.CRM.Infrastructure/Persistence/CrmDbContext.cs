using Microsoft.EntityFrameworkCore;
using SpeededERP.BuildingBlocks.Infrastructure.Persistence;
using SpeededERP.Modules.Business.CRM.Domain.Aggregates.Clients;
using SpeededERP.Modules.Business.CRM.Domain.Aggregates.Devis;
using SpeededERP.Modules.Business.CRM.Infrastructure.Persistence.Configurations;

namespace SpeededERP.Modules.Business.CRM.Infrastructure.Persistence;

public sealed class CrmDbContext(DbContextOptions<CrmDbContext> options) : AppDbContextBase(options)
{
    protected override string ModuleSchema => "crm";

    public DbSet<Client> Clients => Set<Client>();

    public DbSet<Devis> DevisSet => Set<Devis>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ClientConfiguration());
        modelBuilder.ApplyConfiguration(new DevisConfiguration());
    }
}
