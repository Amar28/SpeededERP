using System.Text.RegularExpressions;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Identity.Domain.ValueObjects;

public sealed partial class EmailAddress : ValueObject
{
    public string Value { get; }

    private EmailAddress(string value)
    {
        Value = value;
    }

    public static Result<EmailAddress> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<EmailAddress>(Error.Validation("Email.Empty", "L'adresse e-mail est requise."));
        }

        var normalized = value.Trim().ToLowerInvariant();

        if (!EmailRegex().IsMatch(normalized))
        {
            return Result.Failure<EmailAddress>(Error.Validation("Email.Invalid", "L'adresse e-mail n'est pas valide."));
        }

        return Result.Success(new EmailAddress(normalized));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
