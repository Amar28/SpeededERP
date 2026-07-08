using Ardalis.GuardClauses;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Workflow.Domain.Aggregates.WorkflowDefinitions;

/// <summary>
/// A business-process automation definition (Elsa-compatible workflow JSON, architecture
/// section 12), optionally bound to a metadata entity so it can be triggered from record
/// lifecycle events. The backend only persists and validates the JSON is well-formed —
/// the workflow shape itself is owned by the Elsa designer/runtime.
/// </summary>
public sealed class WorkflowDefinition : AggregateRoot<Guid>, IAuditable, IMustHaveTenant
{
    public string Name { get; private set; } = null!;

    public Guid? EntityDefinitionId { get; private set; }

    public string DefinitionJson { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public Guid TenantId { get; private set; }

    public DateTimeOffset CreatedOnUtc { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTimeOffset? LastModifiedOnUtc { get; private set; }

    public string? LastModifiedBy { get; private set; }

    private WorkflowDefinition()
    {
    }

    private WorkflowDefinition(Guid id, string name, Guid? entityDefinitionId, string definitionJson)
        : base(id)
    {
        Name = name;
        EntityDefinitionId = entityDefinitionId;
        DefinitionJson = definitionJson;
    }

    public static Result<WorkflowDefinition> Create(string name, Guid? entityDefinitionId, string definitionJson)
    {
        Guard.Against.NullOrWhiteSpace(name);
        Guard.Against.NullOrWhiteSpace(definitionJson);

        if (!IsWellFormedJson(definitionJson))
        {
            return Result.Failure<WorkflowDefinition>(Error.Validation("WorkflowDefinition.InvalidDefinition", "La définition du workflow n'est pas un JSON valide."));
        }

        return Result.Success(new WorkflowDefinition(Guid.NewGuid(), name, entityDefinitionId, definitionJson));
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

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
