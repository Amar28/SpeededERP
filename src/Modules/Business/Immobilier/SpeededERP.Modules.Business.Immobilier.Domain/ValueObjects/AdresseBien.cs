using Ardalis.GuardClauses;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.Immobilier.Domain.ValueObjects;

/// <summary>
/// Local to Immobilier — modules never reference each other's Domain assemblies, so this
/// mirrors CRM's <c>Address</c> value object rather than being shared (architecture section 07).
/// </summary>
public sealed class AdresseBien : ValueObject
{
    public string Street { get; }

    public string City { get; }

    public string PostalCode { get; }

    public string CountryIso2 { get; }

    private AdresseBien(string street, string city, string postalCode, string countryIso2)
    {
        Street = street;
        City = city;
        PostalCode = postalCode;
        CountryIso2 = countryIso2;
    }

    public static Result<AdresseBien> Create(string street, string city, string postalCode, string countryIso2)
    {
        Guard.Against.NullOrWhiteSpace(street);
        Guard.Against.NullOrWhiteSpace(city);
        Guard.Against.NullOrWhiteSpace(postalCode);

        if (string.IsNullOrWhiteSpace(countryIso2) || countryIso2.Length != 2)
        {
            return Result.Failure<AdresseBien>(Error.Validation("AdresseBien.InvalidCountry", "Le pays doit être un code ISO 3166-1 alpha-2."));
        }

        return Result.Success(new AdresseBien(street.Trim(), city.Trim(), postalCode.Trim(), countryIso2.ToUpperInvariant()));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return PostalCode;
        yield return CountryIso2;
    }

    public override string ToString() => $"{Street}, {PostalCode} {City}, {CountryIso2}";
}
