using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Collaboration.Domain.Aggregates.Comments;

namespace SpeededERP.Modules.Core.Collaboration.Domain.Repositories;

public interface ICommentRepository : IRepository<Comment, Guid>
{
    /// <summary>The activity-feed read for a given record — ordered oldest first (architecture section 05).</summary>
    Task<IReadOnlyCollection<Comment>> GetByEntityAsync(
        string entityType,
        Guid entityId,
        Guid tenantId,
        CancellationToken cancellationToken = default);
}
