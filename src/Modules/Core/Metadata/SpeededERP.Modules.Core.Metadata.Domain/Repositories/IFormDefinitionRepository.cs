using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Metadata.Domain.Aggregates.FormDefinitions;

namespace SpeededERP.Modules.Core.Metadata.Domain.Repositories;

public interface IFormDefinitionRepository : IRepository<FormDefinition, Guid>
{
    Task<IReadOnlyCollection<FormDefinition>> GetByEntityDefinitionAsync(
        Guid entityDefinitionId,
        Guid tenantId,
        CancellationToken cancellationToken = default);
}
