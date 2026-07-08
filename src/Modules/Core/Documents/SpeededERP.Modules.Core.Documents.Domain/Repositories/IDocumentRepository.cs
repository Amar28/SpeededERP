using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Documents.Domain.Aggregates.Documents;

namespace SpeededERP.Modules.Core.Documents.Domain.Repositories;

public interface IDocumentRepository : IRepository<Document, Guid>
{
}
