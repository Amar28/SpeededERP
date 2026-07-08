using Ardalis.GuardClauses;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Metadata.Domain.Aggregates.FormDefinitions;

/// <summary>
/// A drag-and-drop form layout (sections/rows/columns bound to native or custom
/// fields), edited in the Admin Portal and rendered client-side by the React
/// FormRenderer against a widget registry (architecture section 11). The JSON schema
/// itself is owned by the frontend <c>form-engine</c> package; the backend only
/// persists, versions and validates against it structurally.
/// </summary>
public sealed class FormDefinition : AggregateRoot<Guid>, IAuditable, IMustHaveTenant
{
    public Guid EntityDefinitionId { get; private set; }

    public string Name { get; private set; } = null!;

    public string LayoutJson { get; private set; } = null!;

    public int Version { get; private set; }

    public bool IsPublished { get; private set; }

    public Guid TenantId { get; private set; }

    public DateTimeOffset CreatedOnUtc { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTimeOffset? LastModifiedOnUtc { get; private set; }

    public string? LastModifiedBy { get; private set; }

    private FormDefinition()
    {
    }

    private FormDefinition(Guid id, Guid entityDefinitionId, string name, string layoutJson)
        : base(id)
    {
        EntityDefinitionId = entityDefinitionId;
        Name = name;
        LayoutJson = layoutJson;
        Version = 1;
    }

    public static Result<FormDefinition> Create(Guid entityDefinitionId, string name, string layoutJson)
    {
        Guard.Against.Default(entityDefinitionId);
        Guard.Against.NullOrWhiteSpace(name);
        Guard.Against.NullOrWhiteSpace(layoutJson);

        if (!IsWellFormedJson(layoutJson))
        {
            return Result.Failure<FormDefinition>(Error.Validation("FormDefinition.InvalidLayout", "Le layout du formulaire n'est pas un JSON valide."));
        }

        return Result.Success(new FormDefinition(Guid.NewGuid(), entityDefinitionId, name, layoutJson));
    }

    public Result UpdateLayout(string layoutJson)
    {
        if (!IsWellFormedJson(layoutJson))
        {
            return Result.Failure(Error.Validation("FormDefinition.InvalidLayout", "Le layout du formulaire n'est pas un JSON valide."));
        }

        LayoutJson = layoutJson;
        Version++;
        IsPublished = false;
        return Result.Success();
    }

    public void Publish() => IsPublished = true;

    private static bool IsWellFormedJson(string json)
    {
        try
        {
            using var _ = System.Text.Json.JsonDocument.Parse(json);
            return true;
        }
        catch (System.Text.Json.JsonException)
        {
            return false;
        }
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
