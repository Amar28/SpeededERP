using Microsoft.EntityFrameworkCore;
using SpeededERP.Modules.Core.Metadata.Domain.Aggregates.EntityDefinitions;
using SpeededERP.Modules.Core.Metadata.Domain.Repositories;

namespace SpeededERP.Modules.Core.Metadata.Infrastructure.Persistence.Repositories;

public sealed class EntityDefinitionRepository(MetadataDbContext dbContext) : IEntityDefinitionRepository
{
    public Task<EntityDefinition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.EntityDefinitions.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public Task<EntityDefinition?> GetByModuleAndEntityAsync(string moduleName, string entityName, CancellationToken cancellationToken = default) =>
        dbContext.EntityDefinitions.FirstOrDefaultAsync(e => e.ModuleName == moduleName && e.EntityName == entityName, cancellationToken);

    public void Add(EntityDefinition aggregate) => dbContext.EntityDefinitions.Add(aggregate);

    public void Update(EntityDefinition aggregate) => dbContext.EntityDefinitions.Update(aggregate);

    public void Remove(EntityDefinition aggregate) => dbContext.EntityDefinitions.Remove(aggregate);
}
