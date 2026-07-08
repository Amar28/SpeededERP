using Microsoft.EntityFrameworkCore;
using SpeededERP.BuildingBlocks.Infrastructure.Persistence;
using SpeededERP.Modules.Core.Documents.Domain.Aggregates.Documents;
using SpeededERP.Modules.Core.Documents.Infrastructure.Persistence.Configurations;

namespace SpeededERP.Modules.Core.Documents.Infrastructure.Persistence;

public sealed class DocumentsDbContext(DbContextOptions<DocumentsDbContext> options) : AppDbContextBase(options)
{
    protected override string ModuleSchema => "documents";

    public DbSet<Document> Documents => Set<Document>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new DocumentConfiguration());
    }
}
