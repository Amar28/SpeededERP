using Microsoft.EntityFrameworkCore;
using SpeededERP.Modules.Core.Collaboration.Domain.Aggregates.Comments;
using SpeededERP.Modules.Core.Collaboration.Domain.Repositories;

namespace SpeededERP.Modules.Core.Collaboration.Infrastructure.Persistence.Repositories;

public sealed class CommentRepository(CollaborationDbContext dbContext) : ICommentRepository
{
    public Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Comments.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<Comment>> GetByEntityAsync(
        string entityType,
        Guid entityId,
        Guid tenantId,
        CancellationToken cancellationToken = default) =>
        await dbContext.Comments
            .Where(c => c.EntityType == entityType && c.EntityId == entityId && c.TenantId == tenantId)
            .OrderBy(c => c.CreatedOnUtc)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public void Add(Comment aggregate) => dbContext.Comments.Add(aggregate);

    public void Update(Comment aggregate) => dbContext.Comments.Update(aggregate);

    public void Remove(Comment aggregate) => dbContext.Comments.Remove(aggregate);
}
