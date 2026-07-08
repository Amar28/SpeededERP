using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Identity.Domain.ValueObjects;

/// <summary>
/// Atomic "ressource:action" permission, e.g. "crm:devis:accept" or
/// "metadata:manage-forms" (architecture section 18).
/// </summary>
public sealed class PermissionCode : ValueObject
{
    public string Value { get; }

    private PermissionCode(string value)
    {
        Value = value;
    }

    public static Result<PermissionCode> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<PermissionCode>(Error.Validation("Permission.Empty", "Le code de permission est requis."));
        }

        var normalized = value.Trim().ToLowerInvariant();

        if (!normalized.Contains(':'))
        {
            return Result.Failure<PermissionCode>(
                Error.Validation("Permission.InvalidFormat", "Le code de permission doit suivre le format 'ressource:action'."));
        }

        return Result.Success(new PermissionCode(normalized));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
