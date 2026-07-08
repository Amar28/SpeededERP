namespace SpeededERP.Modules.Core.Collaboration.Presentation.Contracts;

public sealed record PostCommentRequest(string EntityType, Guid EntityId, Guid AuthorUserId, string Body);
