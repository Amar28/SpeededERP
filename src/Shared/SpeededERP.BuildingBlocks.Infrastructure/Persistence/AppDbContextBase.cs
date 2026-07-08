using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.BuildingBlocks.Infrastructure.Outbox;
using SpeededERP.BuildingBlocks.Messaging;

namespace SpeededERP.BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// Base DbContext every module inherits from (one per module/schema, section 06).
/// Wires the Outbox table and exposes <see cref="IOutboxWriter"/> so Application-layer
/// command handlers can enqueue an IntegrationEvent inside the same unit of work as the
/// aggregate change — see architecture section 09, Fig. 03.
/// Audit stamping, tenant assignment and domain-event dispatch happen in
/// <see cref="AuditableEntitiesInterceptor"/> and <see cref="DomainEventsInterceptor"/>,
/// registered by each module's Infrastructure DI extension.
/// </summary>
public abstract class AppDbContextBase(DbContextOptions options) : DbContext(options), IOutboxDbContext, IUnitOfWork
{
    /// <summary>Fixed schema for the outbox table, distinct from the module's business schema.</summary>
    protected abstract string ModuleSchema { get; }

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public Task EnqueueAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        var message = new OutboxMessage
        {
            Id = integrationEvent.EventId,
            Type = integrationEvent.GetType().AssemblyQualifiedName!,
            Content = JsonSerializer.Serialize(integrationEvent, integrationEvent.GetType()),
            TenantId = integrationEvent.TenantId,
            OccurredOnUtc = integrationEvent.OccurredOnUtc,
        };

        OutboxMessages.Add(message);
        return Task.CompletedTask;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(ModuleSchema);

        modelBuilder.Entity<OutboxMessage>(builder =>
        {
            builder.ToTable("outbox_messages", ModuleSchema);
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Type).HasMaxLength(1024).IsRequired();
            builder.Property(m => m.Content).HasColumnType("jsonb").IsRequired();
            builder.HasIndex(m => m.ProcessedOnUtc);
        });

        base.OnModelCreating(modelBuilder);
    }
}
