using Microsoft.EntityFrameworkCore;
using SpeededERP.Modules.Core.Identity.Domain.Aggregates.Roles;
using SpeededERP.Modules.Core.Identity.Domain.Repositories;

namespace SpeededERP.Modules.Core.Identity.Infrastructure.Persistence.Repositories;

public sealed class RoleRepository(IdentityModuleDbContext dbContext) : IRoleRepository
{
    public Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.BusinessRoles.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<Role>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        if (idList.Count == 0)
        {
            return [];
        }

        return await dbContext.BusinessRoles.Where(r => idList.Contains(r.Id)).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public void Add(Role aggregate) => dbContext.BusinessRoles.Add(aggregate);

    public void Update(Role aggregate) => dbContext.BusinessRoles.Update(aggregate);

    public void Remove(Role aggregate) => dbContext.BusinessRoles.Remove(aggregate);
}
