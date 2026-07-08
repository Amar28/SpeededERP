using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Core.Collaboration.Application.Comments.Queries.GetCommentsByEntity;

/// <summary>The activity-feed read for a given record (architecture section 05).</summary>
public sealed record GetCommentsByEntityQuery(string EntityType, Guid EntityId, Guid TenantId)
    : IQuery<IReadOnlyCollection<CommentDto>>;
