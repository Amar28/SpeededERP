using Microsoft.EntityFrameworkCore;
using SpeededERP.Modules.Business.Immobilier.Domain.Aggregates.Biens;
using SpeededERP.Modules.Business.Immobilier.Domain.Repositories;
using SpeededERP.Modules.Business.Immobilier.Infrastructure.Persistence;

namespace SpeededERP.Modules.Business.Immobilier.Infrastructure.Persistence.Repositories;

public sealed class BienRepository(ImmobilierDbContext dbContext) : IBienRepository
{
    public Task<Bien?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Biens.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public void Add(Bien aggregate) => dbContext.Biens.Add(aggregate);

    public void Update(Bien aggregate) => dbContext.Biens.Update(aggregate);

    public void Remove(Bien aggregate) => dbContext.Biens.Remove(aggregate);
}
