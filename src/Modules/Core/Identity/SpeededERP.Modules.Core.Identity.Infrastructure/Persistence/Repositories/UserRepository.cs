using Microsoft.EntityFrameworkCore;
using SpeededERP.Modules.Core.Identity.Domain.Aggregates.Users;
using SpeededERP.Modules.Core.Identity.Domain.Repositories;
using SpeededERP.Modules.Core.Identity.Domain.ValueObjects;

namespace SpeededERP.Modules.Core.Identity.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(IdentityModuleDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.BusinessUsers.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<User?> GetByEmailAsync(EmailAddress email, Guid tenantId, CancellationToken cancellationToken = default) =>
        dbContext.BusinessUsers.FirstOrDefaultAsync(
            u => u.Email == email && u.TenantId == tenantId,
            cancellationToken);

    public Task<bool> ExistsWithEmailAsync(EmailAddress email, Guid tenantId, CancellationToken cancellationToken = default) =>
        dbContext.BusinessUsers.AnyAsync(u => u.Email == email && u.TenantId == tenantId, cancellationToken);

    public void Add(User aggregate) => dbContext.BusinessUsers.Add(aggregate);

    public void Update(User aggregate) => dbContext.BusinessUsers.Update(aggregate);

    public void Remove(User aggregate) => dbContext.BusinessUsers.Remove(aggregate);
}
