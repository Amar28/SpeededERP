using Ardalis.GuardClauses;

namespace SpeededERP.SharedKernel;

/// <summary>
/// Immutable monetary amount with an ISO 4217 currency — the "Montant" value object
/// used throughout every module's Domain layer (architecture section 27). Never mix
/// amounts of different currencies; convert explicitly via IExchangeRateProvider first.
/// </summary>
public sealed class Money : ValueObject
{
    public decimal Amount { get; }

    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Of(decimal amount, string currencyIso4217)
    {
        Guard.Against.NullOrWhiteSpace(currencyIso4217);
        if (currencyIso4217.Length != 3)
        {
            throw new ArgumentException("La devise doit être un code ISO 4217 à 3 lettres.", nameof(currencyIso4217));
        }

        return new Money(amount, currencyIso4217.ToUpperInvariant());
    }

    public static Money Zero(string currencyIso4217) => Of(0m, currencyIso4217);

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal factor) => new(Amount * factor, Currency);

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new InvalidOperationException(
                $"Impossible d'opérer sur deux montants de devises différentes ({Currency} et {other.Currency}).");
        }
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:0.00} {Currency}";

    public static Money operator +(Money left, Money right) => left.Add(right);

    public static Money operator -(Money left, Money right) => left.Subtract(right);
}
