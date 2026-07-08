using Microsoft.EntityFrameworkCore;
using SpeededERP.BuildingBlocks.Infrastructure.Persistence;
using SpeededERP.Modules.Core.Config.Infrastructure.Persistence.Configurations;
using TenantSettingsAggregate = SpeededERP.Modules.Core.Config.Domain.Aggregates.TenantSettings.TenantSettings;

namespace SpeededERP.Modules.Core.Config.Infrastructure.Persistence;

public sealed class ConfigDbContext(DbContextOptions<ConfigDbContext> options) : AppDbContextBase(options)
{
    protected override string ModuleSchema => "config";

    public DbSet<TenantSettingsAggregate> TenantSettingsSet => Set<TenantSettingsAggregate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new TenantSettingsConfiguration());
    }
}
