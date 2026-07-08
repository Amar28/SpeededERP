using Microsoft.EntityFrameworkCore;
using SpeededERP.Modules.Business.Logistique.Domain.Aggregates.Expeditions;
using SpeededERP.Modules.Business.Logistique.Domain.Repositories;
using SpeededERP.Modules.Business.Logistique.Infrastructure.Persistence;

namespace SpeededERP.Modules.Business.Logistique.Infrastructure.Persistence.Repositories;

public sealed class ExpeditionRepository(LogistiqueDbContext dbContext) : IExpeditionRepository
{
    public Task<Expedition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Expeditions.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public void Add(Expedition aggregate) => dbContext.Expeditions.Add(aggregate);

    public void Update(Expedition aggregate) => dbContext.Expeditions.Update(aggregate);

    public void Remove(Expedition aggregate) => dbContext.Expeditions.Remove(aggregate);
}
