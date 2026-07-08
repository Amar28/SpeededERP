using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SpeededERP.BuildingBlocks.Application.Security;
using SpeededERP.SharedKernel;

namespace SpeededERP.BuildingBlocks.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Stamps <see cref="IAuditable"/> and <see cref="IMustHaveTenant"/> on every tracked
/// entity right before it is persisted (architecture section 23) — never set manually
/// from Application handlers.
/// </summary>
public sealed class AuditableEntitiesInterceptor(ICurrentUserContext currentUser, ITenantContext tenantContext)
    : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Apply(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        Apply(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Apply(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCreated(now, currentUser.UserId?.ToString());
                    break;
                case EntityState.Modified:
                    entry.Entity.SetModified(now, currentUser.UserId?.ToString());
                    break;
            }
        }

        foreach (var entry in context.ChangeTracker.Entries<IMustHaveTenant>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.AssignTenant(tenantContext.TenantId);
            }
        }

        foreach (var entry in context.ChangeTracker.Entries<ISoftDeletable>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.MarkDeleted(now);
            }
        }
    }
}
