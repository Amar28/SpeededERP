using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Config.Domain.Aggregates.TenantSettings;

namespace SpeededERP.Modules.Core.Config.Domain.Repositories;

public interface ITenantSettingsRepository : IRepository<TenantSettings, Guid>
{
    /// <summary>Semantically the same lookup as <see cref="IRepository{TAggregate,TId}.GetByIdAsync"/> since Id == TenantId — kept as a distinct method for read-site clarity.</summary>
    Task<TenantSettings?> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
