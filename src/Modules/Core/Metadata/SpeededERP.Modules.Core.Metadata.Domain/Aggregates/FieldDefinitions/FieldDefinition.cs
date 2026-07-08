using Ardalis.GuardClauses;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Metadata.Domain.Aggregates.FieldDefinitions;

/// <summary>
/// A tenant-added custom field on an <c>EntityDefinition</c> (architecture section 10):
/// key, label, type, validation rules, required flag, display order. Stored separately
/// from the native, code-owned fields — the two coexist on the same runtime table via
/// the <c>custom_fields JSONB</c> column, but only custom fields are described here.
/// </summary>
public sealed class FieldDefinition : AggregateRoot<Guid>, IAuditable, IMustHaveTenant
{
    private readonly List<string> _listOptions = [];

    public Guid EntityDefinitionId { get; private set; }

    public string Key { get; private set; } = null!;

    public string LabelKey { get; private set; } = null!;

    public FieldType Type { get; private set; }

    public bool IsRequired { get; private set; }

    public int Order { get; private set; }

    /// <summary>Declarative JsonLogic validation rule (section 13) — never arbitrary code.</summary>
    public string? ValidationRuleJson { get; private set; }

    public IReadOnlyCollection<string> ListOptions => _listOptions;

    public Guid TenantId { get; private set; }

    public DateTimeOffset CreatedOnUtc { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTimeOffset? LastModifiedOnUtc { get; private set; }

    public string? LastModifiedBy { get; private set; }

    private FieldDefinition()
    {
    }

    private FieldDefinition(Guid id, Guid entityDefinitionId, string key, string labelKey, FieldType type, int order)
        : base(id)
    {
        EntityDefinitionId = entityDefinitionId;
        Key = key;
        LabelKey = labelKey;
        Type = type;
        Order = order;
    }

    public static Result<FieldDefinition> Create(
        Guid entityDefinitionId,
        string key,
        string labelKey,
        FieldType type,
        int order)
    {
        Guard.Against.Default(entityDefinitionId);
        Guard.Against.NullOrWhiteSpace(key);
        Guard.Against.NullOrWhiteSpace(labelKey);

        if (!System.Text.RegularExpressions.Regex.IsMatch(key, "^[a-z][a-z0-9_]{1,63}$"))
        {
            return Result.Failure<FieldDefinition>(Error.Validation(
                "FieldDefinition.InvalidKey",
                "La clé du champ doit être en minuscules, chiffres et underscores (2 à 64 caractères)."));
        }

        return Result.Success(new FieldDefinition(Guid.NewGuid(), entityDefinitionId, key, labelKey, type, order));
    }

    public void MarkRequired(bool isRequired) => IsRequired = isRequired;

    public void Reorder(int order) => Order = order;

    public Result SetValidationRule(string? jsonLogicRule)
    {
        ValidationRuleJson = jsonLogicRule;
        return Result.Success();
    }

    public Result SetListOptions(IEnumerable<string> options)
    {
        if (Type != FieldType.List)
        {
            return Result.Failure(Error.Validation(
                "FieldDefinition.NotAList",
                "Les options ne sont applicables qu'aux champs de type Liste."));
        }

        _listOptions.Clear();
        _listOptions.AddRange(options);
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
