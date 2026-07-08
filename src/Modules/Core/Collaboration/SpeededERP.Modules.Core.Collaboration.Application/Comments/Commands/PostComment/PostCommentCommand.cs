using SpeededERP.BuildingBlocks.Application.Messaging;
using SpeededERP.BuildingBlocks.Application.Security;

namespace SpeededERP.Modules.Core.Collaboration.Application.Comments.Commands.PostComment;

/// <summary>Posts a new comment on the activity feed of any record (architecture section 05).</summary>
[RequirePermission("collaboration:post-comments")]
public sealed record PostCommentCommand(
    string EntityType,
    Guid EntityId,
    Guid AuthorUserId,
    string Body) : ICommand<Guid>;
