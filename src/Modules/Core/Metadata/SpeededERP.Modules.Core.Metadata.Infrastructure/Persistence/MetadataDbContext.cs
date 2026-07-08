using Microsoft.EntityFrameworkCore;
using SpeededERP.BuildingBlocks.Infrastructure.Persistence;
using SpeededERP.Modules.Core.Metadata.Domain.Aggregates.EntityDefinitions;
using SpeededERP.Modules.Core.Metadata.Domain.Aggregates.FieldDefinitions;
using SpeededERP.Modules.Core.Metadata.Domain.Aggregates.FormDefinitions;
using SpeededERP.Modules.Core.Metadata.Infrastructure.Persistence.Configurations;

namespace SpeededERP.Modules.Core.Metadata.Infrastructure.Persistence;

public sealed class MetadataDbContext(DbContextOptions<MetadataDbContext> options) : AppDbContextBase(options)
{
    protected override string ModuleSchema => "metadata";

    public DbSet<EntityDefinition> EntityDefinitions => Set<EntityDefinition>();

    public DbSet<FieldDefinition> FieldDefinitions => Set<FieldDefinition>();

    public DbSet<FormDefinition> FormDefinitions => Set<FormDefinition>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new EntityDefinitionConfiguration());
        modelBuilder.ApplyConfiguration(new FieldDefinitionConfiguration());
        modelBuilder.ApplyConfiguration(new FormDefinitionConfiguration());
    }
}
