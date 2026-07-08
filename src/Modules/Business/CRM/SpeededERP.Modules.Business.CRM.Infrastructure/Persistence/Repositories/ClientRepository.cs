using Microsoft.EntityFrameworkCore;
using SpeededERP.Modules.Business.CRM.Domain.Aggregates.Clients;
using SpeededERP.Modules.Business.CRM.Domain.Repositories;

namespace SpeededERP.Modules.Business.CRM.Infrastructure.Persistence.Repositories;

public sealed class ClientRepository(CrmDbContext dbContext) : IClientRepository
{
    public Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Clients.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public void Add(Client aggregate) => dbContext.Clients.Add(aggregate);

    public void Update(Client aggregate) => dbContext.Clients.Update(aggregate);

    public void Remove(Client aggregate) => dbContext.Clients.Remove(aggregate);
}
