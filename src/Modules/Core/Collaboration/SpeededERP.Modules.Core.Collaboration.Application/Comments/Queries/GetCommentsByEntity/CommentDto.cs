namespace SpeededERP.Modules.Core.Collaboration.Application.Comments.Queries.GetCommentsByEntity;

public sealed record CommentDto(
    Guid Id,
    string EntityType,
    Guid EntityId,
    Guid AuthorUserId,
    string Body,
    bool IsEdited,
    DateTimeOffset CreatedOnUtc);
