using Ardalis.GuardClauses;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.CRM.Domain.ValueObjects;

public sealed class Address : ValueObject
{
    public string Street { get; }

    public string City { get; }

    public string PostalCode { get; }

    public string CountryIso2 { get; }

    private Address(string street, string city, string postalCode, string countryIso2)
    {
        Street = street;
        City = city;
        PostalCode = postalCode;
        CountryIso2 = countryIso2;
    }

    public static Result<Address> Create(string street, string city, string postalCode, string countryIso2)
    {
        Guard.Against.NullOrWhiteSpace(street);
        Guard.Against.NullOrWhiteSpace(city);
        Guard.Against.NullOrWhiteSpace(postalCode);

        if (string.IsNullOrWhiteSpace(countryIso2) || countryIso2.Length != 2)
        {
            return Result.Failure<Address>(Error.Validation("Address.InvalidCountry", "Le pays doit être un code ISO 3166-1 alpha-2."));
        }

        return Result.Success(new Address(street.Trim(), city.Trim(), postalCode.Trim(), countryIso2.ToUpperInvariant()));
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
