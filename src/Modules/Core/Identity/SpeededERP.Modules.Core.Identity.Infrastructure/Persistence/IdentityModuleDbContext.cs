using System.Text.Json;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.BuildingBlocks.Infrastructure.Outbox;
using SpeededERP.BuildingBlocks.Messaging;
using SpeededERP.Modules.Core.Identity.Domain.Aggregates.Roles;
using SpeededERP.Modules.Core.Identity.Domain.Aggregates.Tenants;
using SpeededERP.Modules.Core.Identity.Domain.Aggregates.Users;
using SpeededERP.Modules.Core.Identity.Infrastructure.Identity;
using SpeededERP.Modules.Core.Identity.Infrastructure.Persistence.Configurations;

namespace SpeededERP.Modules.Core.Identity.Infrastructure.Persistence;

/// <summary>
/// The one exception to inheriting <c>AppDbContextBase</c> (see architecture section
/// 06 note on Core.Identity): ASP.NET Core Identity requires
/// <see cref="IdentityDbContext{TUser,TRole,TKey}"/> as its base class, which cannot be
/// combined with our own DbContext base. The Outbox plumbing that
/// <c>AppDbContextBase</c> normally provides is therefore duplicated here explicitly.
/// </summary>
public sealed class IdentityModuleDbContext(DbContextOptions<IdentityModuleDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options), IOutboxDbContext, IUnitOfWork
{
    public const string Schema = "identity";

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public DbSet<Domain.Aggregates.Users.User> BusinessUsers => Set<Domain.Aggregates.Users.User>();

    public DbSet<Role> BusinessRoles => Set<Role>();

    public DbSet<Tenant> Tenants => Set<Tenant>();

    public Task EnqueueAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        OutboxMessages.Add(new OutboxMessage
        {
            Id = integrationEvent.EventId,
            Type = integrationEvent.GetType().AssemblyQualifiedName!,
            Content = JsonSerializer.Serialize(integrationEvent, integrationEvent.GetType()),
            TenantId = integrationEvent.TenantId,
            OccurredOnUtc = integrationEvent.OccurredOnUtc,
        });

        return Task.CompletedTask;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema(Schema);

        builder.Entity<OutboxMessage>(cfg =>
        {
            cfg.ToTable("outbox_messages", Schema);
            cfg.HasKey(m => m.Id);
            cfg.Property(m => m.Type).HasMaxLength(1024).IsRequired();
            cfg.Property(m => m.Content).HasColumnType("jsonb").IsRequired();
            cfg.HasIndex(m => m.ProcessedOnUtc);
        });

        builder.ApplyConfiguration(new UserConfiguration());
        builder.ApplyConfiguration(new RoleConfiguration());
        builder.ApplyConfiguration(new TenantConfiguration());
    }
}
