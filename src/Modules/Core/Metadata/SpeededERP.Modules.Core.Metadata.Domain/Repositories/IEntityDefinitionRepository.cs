using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Metadata.Domain.Aggregates.EntityDefinitions;

namespace SpeededERP.Modules.Core.Metadata.Domain.Repositories;

public interface IEntityDefinitionRepository : IRepository<EntityDefinition, Guid>
{
    Task<EntityDefinition?> GetByModuleAndEntityAsync(string moduleName, string entityName, CancellationToken cancellationToken = default);
}
