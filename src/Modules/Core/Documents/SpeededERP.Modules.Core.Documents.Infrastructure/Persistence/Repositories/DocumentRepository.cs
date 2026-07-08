using Microsoft.EntityFrameworkCore;
using SpeededERP.Modules.Core.Documents.Domain.Aggregates.Documents;
using SpeededERP.Modules.Core.Documents.Domain.Repositories;

namespace SpeededERP.Modules.Core.Documents.Infrastructure.Persistence.Repositories;

public sealed class DocumentRepository(DocumentsDbContext dbContext) : IDocumentRepository
{
    public Task<Document?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Documents.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public void Add(Document aggregate) => dbContext.Documents.Add(aggregate);

    public void Update(Document aggregate) => dbContext.Documents.Update(aggregate);

    public void Remove(Document aggregate) => dbContext.Documents.Remove(aggregate);
}
