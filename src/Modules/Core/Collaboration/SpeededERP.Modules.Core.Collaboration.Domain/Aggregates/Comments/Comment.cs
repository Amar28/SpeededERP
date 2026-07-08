using Ardalis.GuardClauses;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Collaboration.Domain.Aggregates.Comments;

/// <summary>
/// A single entry in the activity feed attached to any record across the platform
/// (architecture section 05, "fil d'activité sur tout enregistrement"). EntityType/EntityId
/// identify the owning aggregate (e.g. "CRM.Client") without a hard foreign key, since
/// Collaboration must stay decoupled from every business module it can comment on.
/// </summary>
public sealed class Comment : AggregateRoot<Guid>, IAuditable, IMustHaveTenant
{
    private const int MaxBodyLength = 4000;

    public string EntityType { get; private set; } = null!;

    public Guid EntityId { get; private set; }

    public Guid AuthorUserId { get; private set; }

    public string Body { get; private set; } = null!;

    public bool IsEdited { get; private set; }

    public Guid TenantId { get; private set; }

    public DateTimeOffset CreatedOnUtc { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTimeOffset? LastModifiedOnUtc { get; private set; }

    public string? LastModifiedBy { get; private set; }

    private Comment()
    {
    }

    private Comment(Guid id, string entityType, Guid entityId, Guid authorUserId, string body)
        : base(id)
    {
        EntityType = entityType;
        EntityId = entityId;
        AuthorUserId = authorUserId;
        Body = body;
    }

    public static Result<Comment> Post(string entityType, Guid entityId, Guid authorUserId, string body)
    {
        Guard.Against.NullOrWhiteSpace(entityType);
        Guard.Against.Default(entityId);
        Guard.Against.Default(authorUserId);
        Guard.Against.NullOrWhiteSpace(body);

        if (body.Length > MaxBodyLength)
        {
            return Result.Failure<Comment>(Error.Validation("Comment.BodyTooLong", $"Le commentaire dépasse la longueur maximale de {MaxBodyLength} caractères."));
        }

        return Result.Success(new Comment(Guid.NewGuid(), entityType, entityId, authorUserId, body));
    }

    public Result Edit(string newBody)
    {
        if (string.IsNullOrWhiteSpace(newBody))
        {
            return Result.Failure(Error.Validation("Comment.EmptyBody", "Le commentaire ne peut pas être vide."));
        }

        if (newBody.Length > MaxBodyLength)
        {
            return Result.Failure(Error.Validation("Comment.BodyTooLong", $"Le commentaire dépasse la longueur maximale de {MaxBodyLength} caractères."));
        }

        Body = newBody;
        IsEdited = true;
        return Result.Success();
    }

    public void AssignTenant(Guid tenantId) => TenantId = tenantId;

    public void SetCreated(DateTimeOffset onUtc, string? by)
    {
        CreatedOnUtc = onUtc;
        CreatedBy = by;
    }

    public void SetModified(DateTimeOffset onUtc, string? by)
    {
        LastModifiedOnUtc = onUtc;
        LastModifiedBy = by;
    }
}
