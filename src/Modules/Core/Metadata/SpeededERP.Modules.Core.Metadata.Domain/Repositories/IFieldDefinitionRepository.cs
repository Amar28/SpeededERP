using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Metadata.Domain.Aggregates.FieldDefinitions;

namespace SpeededERP.Modules.Core.Metadata.Domain.Repositories;

public interface IFieldDefinitionRepository : IRepository<FieldDefinition, Guid>
{
    Task<IReadOnlyCollection<FieldDefinition>> GetByEntityDefinitionAsync(
        Guid entityDefinitionId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<bool> KeyExistsAsync(Guid entityDefinitionId, Guid tenantId, string key, CancellationToken cancellationToken = default);
}
