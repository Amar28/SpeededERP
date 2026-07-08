using Ardalis.GuardClauses;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Documents.Domain.Aggregates.Documents;

/// <summary>
/// An uploaded file plus its version history (architecture section 21 — "chaque upload
/// crée une version"). The physical bytes live behind the <c>IBlobStorage</c> outbound
/// port (S3/Azure Blob in SaaS, local filesystem in On-Premise); this aggregate only
/// tracks the pointer (<see cref="StorageKey"/>) and metadata.
/// </summary>
public sealed class Document : AggregateRoot<Guid>, IAuditable, IMustHaveTenant, ISoftDeletable
{
    public string FileName { get; private set; } = null!;

    public string StorageKey { get; private set; } = null!;

    public string ContentType { get; private set; } = null!;

    public long SizeBytes { get; private set; }

    public int Version { get; private set; }

    /// <summary>The business record this file is attached to, e.g. "CRM.Client" — null for standalone uploads.</summary>
    public string? EntityType { get; private set; }

    public Guid? EntityId { get; private set; }

    public Guid TenantId { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTimeOffset? DeletedOnUtc { get; private set; }

    public DateTimeOffset CreatedOnUtc { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTimeOffset? LastModifiedOnUtc { get; private set; }

    public string? LastModifiedBy { get; private set; }

    private Document()
    {
    }

    private Document(Guid id, string fileName, string storageKey, string contentType, long sizeBytes, string? entityType, Guid? entityId)
        : base(id)
    {
        FileName = fileName;
        StorageKey = storageKey;
        ContentType = contentType;
        SizeBytes = sizeBytes;
        EntityType = entityType;
        EntityId = entityId;
        Version = 1;
    }

    public static Result<Document> Upload(
        string fileName,
        string storageKey,
        string contentType,
        long sizeBytes,
        string? entityType,
        Guid? entityId)
    {
        Guard.Against.NullOrWhiteSpace(fileName);
        Guard.Against.NullOrWhiteSpace(storageKey);
        Guard.Against.NullOrWhiteSpace(contentType);
        Guard.Against.Negative(sizeBytes);

        return Result.Success(new Document(Guid.NewGuid(), fileName.Trim(), storageKey, contentType, sizeBytes, entityType, entityId));
    }

    /// <summary>Records a new upload of the same logical document — each upload creates a version, never overwrites one.</summary>
    public void CreateNewVersion(string newStorageKey, long newSizeBytes)
    {
        Guard.Against.NullOrWhiteSpace(newStorageKey);
        Guard.Against.Negative(newSizeBytes);

        StorageKey = newStorageKey;
        SizeBytes = newSizeBytes;
        Version++;
    }

    public void AssignTenant(Guid tenantId) => TenantId = tenantId;

    public void MarkDeleted(DateTimeOffset onUtc)
    {
        IsDeleted = true;
        DeletedOnUtc = onUtc;
    }

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
