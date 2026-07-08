using Microsoft.EntityFrameworkCore;
using SpeededERP.Modules.Core.Metadata.Domain.Aggregates.FieldDefinitions;
using SpeededERP.Modules.Core.Metadata.Domain.Repositories;

namespace SpeededERP.Modules.Core.Metadata.Infrastructure.Persistence.Repositories;

public sealed class FieldDefinitionRepository(MetadataDbContext dbContext) : IFieldDefinitionRepository
{
    public Task<FieldDefinition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.FieldDefinitions.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<FieldDefinition>> GetByEntityDefinitionAsync(
        Guid entityDefinitionId,
        Guid tenantId,
        CancellationToken cancellationToken = default) =>
        await dbContext.FieldDefinitions
            .Where(f => f.EntityDefinitionId == entityDefinitionId && f.TenantId == tenantId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public Task<bool> KeyExistsAsync(Guid entityDefinitionId, Guid tenantId, string key, CancellationToken cancellationToken = default) =>
        dbContext.FieldDefinitions.AnyAsync(
            f => f.EntityDefinitionId == entityDefinitionId && f.TenantId == tenantId && f.Key == key,
            cancellationToken);

    public void Add(FieldDefinition aggregate) => dbContext.FieldDefinitions.Add(aggregate);

    public void Update(FieldDefinition aggregate) => dbContext.FieldDefinitions.Update(aggregate);

    public void Remove(FieldDefinition aggregate) => dbContext.FieldDefinitions.Remove(aggregate);
}
