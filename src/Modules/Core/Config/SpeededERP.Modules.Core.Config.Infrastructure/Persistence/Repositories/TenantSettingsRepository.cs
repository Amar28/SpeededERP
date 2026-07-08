using Microsoft.EntityFrameworkCore;
using SpeededERP.Modules.Core.Config.Domain.Repositories;
using TenantSettingsAggregate = SpeededERP.Modules.Core.Config.Domain.Aggregates.TenantSettings.TenantSettings;

namespace SpeededERP.Modules.Core.Config.Infrastructure.Persistence.Repositories;

public sealed class TenantSettingsRepository(ConfigDbContext dbContext) : ITenantSettingsRepository
{
    public Task<TenantSettingsAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.TenantSettingsSet.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public Task<TenantSettingsAggregate?> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        GetByIdAsync(tenantId, cancellationToken);

    public void Add(TenantSettingsAggregate aggregate) => dbContext.TenantSettingsSet.Add(aggregate);

    public void Update(TenantSettingsAggregate aggregate) => dbContext.TenantSettingsSet.Update(aggregate);

    public void Remove(TenantSettingsAggregate aggregate) => dbContext.TenantSettingsSet.Remove(aggregate);
}
