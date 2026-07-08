using Ardalis.GuardClauses;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Metadata.Domain.Aggregates.EntityDefinitions;

/// <summary>
/// Declares that a module owns an entity that can be extended with custom fields,
/// forms, workflows and reports (architecture section 10). Declared by the owning
/// module itself at startup (an <c>EntityDefinitionDeclaredIntegrationEvent</c>
/// consumer upserts it here) — never created directly by a tenant.
/// </summary>
public sealed class EntityDefinition : AggregateRoot<Guid>, IAuditable
{
    public string ModuleName { get; private set; } = null!;

    public string EntityName { get; private set; } = null!;

    public string DisplayNameKey { get; private set; } = null!;

    public bool SupportsCustomFields { get; private set; }

    public DateTimeOffset CreatedOnUtc { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTimeOffset? LastModifiedOnUtc { get; private set; }

    public string? LastModifiedBy { get; private set; }

    private EntityDefinition()
    {
    }

    private EntityDefinition(Guid id, string moduleName, string entityName, string displayNameKey, bool supportsCustomFields)
        : base(id)
    {
        ModuleName = moduleName;
        EntityName = entityName;
        DisplayNameKey = displayNameKey;
        SupportsCustomFields = supportsCustomFields;
    }

    public static Result<EntityDefinition> Declare(
        string moduleName,
        string entityName,
        string displayNameKey,
        bool supportsCustomFields = true)
    {
        Guard.Against.NullOrWhiteSpace(moduleName);
        Guard.Against.NullOrWhiteSpace(entityName);
        Guard.Against.NullOrWhiteSpace(displayNameKey);

        return Result.Success(new EntityDefinition(
            DeterministicId(moduleName, entityName),
            moduleName,
            entityName,
            displayNameKey,
            supportsCustomFields));
    }

    /// <summary>
    /// Deterministic Guid derived from (module, entity) so re-declaring on every
    /// module startup is naturally idempotent (upsert by Id) instead of accumulating
    /// duplicates.
    /// </summary>
    public static Guid DeterministicId(string moduleName, string entityName)
    {
        var input = $"{moduleName.ToLowerInvariant()}::{entityName.ToLowerInvariant()}";
        var hash = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(input));
        return new Guid(hash[..16]);
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
