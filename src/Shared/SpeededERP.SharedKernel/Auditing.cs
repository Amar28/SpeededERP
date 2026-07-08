namespace SpeededERP.SharedKernel;

/// <summary>
/// Implemented by aggregates that must carry standard audit metadata. Populated by
/// the AuditInterceptor in BuildingBlocks.Infrastructure on every SaveChanges call —
/// never set manually from Application handlers.
/// </summary>
public interface IAuditable
{
    DateTimeOffset CreatedOnUtc { get; }

    string? CreatedBy { get; }

    DateTimeOffset? LastModifiedOnUtc { get; }

    string? LastModifiedBy { get; }

    void SetCreated(DateTimeOffset onUtc, string? by);

    void SetModified(DateTimeOffset onUtc, string? by);
}

/// <summary>
/// Implemented by aggregates that support soft delete instead of physical deletion —
/// required for entities referenced by Core.Collaboration history or Audit trails.
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; }

    DateTimeOffset? DeletedOnUtc { get; }

    void MarkDeleted(DateTimeOffset onUtc);
}

/// <summary>
/// Implemented by every aggregate stored in a shared (Row-Level-Security) tenant
/// database — see architecture section 17. The EF Core global query filter and the
/// SaveChangesInterceptor both key off this interface.
/// </summary>
public interface IMustHaveTenant
{
    Guid TenantId { get; }

    void AssignTenant(Guid tenantId);
}
