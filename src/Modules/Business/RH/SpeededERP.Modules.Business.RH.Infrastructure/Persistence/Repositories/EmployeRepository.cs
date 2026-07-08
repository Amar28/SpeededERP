using Microsoft.EntityFrameworkCore;
using SpeededERP.Modules.Business.RH.Domain.Aggregates.Employes;
using SpeededERP.Modules.Business.RH.Domain.Repositories;
using SpeededERP.Modules.Business.RH.Infrastructure.Persistence;

namespace SpeededERP.Modules.Business.RH.Infrastructure.Persistence.Repositories;

public sealed class EmployeRepository(RhDbContext dbContext) : IEmployeRepository
{
    public Task<Employe?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Employes.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public void Add(Employe aggregate) => dbContext.Employes.Add(aggregate);

    public void Update(Employe aggregate) => dbContext.Employes.Update(aggregate);

    public void Remove(Employe aggregate) => dbContext.Employes.Remove(aggregate);
}
