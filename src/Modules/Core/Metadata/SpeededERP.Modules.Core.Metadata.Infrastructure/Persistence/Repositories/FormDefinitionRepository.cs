using Microsoft.EntityFrameworkCore;
using SpeededERP.Modules.Core.Metadata.Domain.Aggregates.FormDefinitions;
using SpeededERP.Modules.Core.Metadata.Domain.Repositories;

namespace SpeededERP.Modules.Core.Metadata.Infrastructure.Persistence.Repositories;

public sealed class FormDefinitionRepository(MetadataDbContext dbContext) : IFormDefinitionRepository
{
    public Task<FormDefinition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.FormDefinitions.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<FormDefinition>> GetByEntityDefinitionAsync(
        Guid entityDefinitionId,
        Guid tenantId,
        CancellationToken cancellationToken = default) =>
        await dbContext.FormDefinitions
            .Where(f => f.EntityDefinitionId == entityDefinitionId && f.TenantId == tenantId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public void Add(FormDefinition aggregate) => dbContext.FormDefinitions.Add(aggregate);

    public void Update(FormDefinition aggregate) => dbContext.FormDefinitions.Update(aggregate);

    public void Remove(FormDefinition aggregate) => dbContext.FormDefinitions.Remove(aggregate);
}
