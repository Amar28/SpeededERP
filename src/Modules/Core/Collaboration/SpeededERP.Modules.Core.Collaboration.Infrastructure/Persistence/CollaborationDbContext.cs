using Microsoft.EntityFrameworkCore;
using SpeededERP.BuildingBlocks.Infrastructure.Persistence;
using SpeededERP.Modules.Core.Collaboration.Domain.Aggregates.Comments;
using SpeededERP.Modules.Core.Collaboration.Infrastructure.Persistence.Configurations;

namespace SpeededERP.Modules.Core.Collaboration.Infrastructure.Persistence;

public sealed class CollaborationDbContext(DbContextOptions<CollaborationDbContext> options) : AppDbContextBase(options)
{
    protected override string ModuleSchema => "collaboration";

    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new CommentConfiguration());
    }
}
