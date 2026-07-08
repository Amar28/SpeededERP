using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Identity.Domain.Aggregates.Users;
using SpeededERP.Modules.Core.Identity.Domain.ValueObjects;

namespace SpeededERP.Modules.Core.Identity.Domain.Repositories;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<User?> GetByEmailAsync(EmailAddress email, Guid tenantId, CancellationToken cancellationToken = default);

    Task<bool> ExistsWithEmailAsync(EmailAddress email, Guid tenantId, CancellationToken cancellationToken = default);
}
