using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Identity.Domain.Aggregates.Roles;

namespace SpeededERP.Modules.Core.Identity.Domain.Repositories;

public interface IRoleRepository : IRepository<Role, Guid>
{
    Task<IReadOnlyCollection<Role>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}
