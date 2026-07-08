using Ardalis.GuardClauses;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Config.Domain.Aggregates.TenantSettings;

/// <summary>
/// Per-tenant configuration read on nearly every request (architecture section 25) —
/// language/currency/timezone defaults and the set of activated modules. One row per
/// tenant: <see cref="Entity{TId}.Id"/> *is* the tenant id, so — unlike most aggregates —
/// this one does not implement <c>IMustHaveTenant</c>; there is no separate tenant column
/// for the interceptor to stamp.
/// </summary>
public sealed class TenantSettings : AggregateRoot<Guid>, IAuditable
{
    private readonly List<string> _activatedModules = [];

    public string DefaultLanguage { get; private set; } = "fr";

    public string DefaultCurrency { get; private set; } = "EUR";

    public string TimeZoneId { get; private set; } = "Europe/Paris";

    public IReadOnlyCollection<string> ActivatedModules => _activatedModules;

    public DateTimeOffset CreatedOnUtc { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTimeOffset? LastModifiedOnUtc { get; private set; }

    public string? LastModifiedBy { get; private set; }

    private TenantSettings()
    {
    }

    private TenantSettings(Guid tenantId, string defaultLanguage, string defaultCurrency, string timeZoneId)
        : base(tenantId)
    {
        DefaultLanguage = defaultLanguage;
        DefaultCurrency = defaultCurrency;
        TimeZoneId = timeZoneId;
    }

    public static Result<TenantSettings> InitializeForTenant(Guid tenantId, string defaultLanguage, string defaultCurrency, string timeZoneId)
    {
        Guard.Against.Default(tenantId);
        Guard.Against.NullOrWhiteSpace(defaultLanguage);
        Guard.Against.NullOrWhiteSpace(defaultCurrency);
        Guard.Against.NullOrWhiteSpace(timeZoneId);

        return Result.Success(new TenantSettings(tenantId, defaultLanguage, defaultCurrency, timeZoneId));
    }

    public void ActivateModule(string moduleName)
    {
        if (!_activatedModules.Contains(moduleName))
        {
            _activatedModules.Add(moduleName);
        }
    }

    public void DeactivateModule(string moduleName) => _activatedModules.Remove(moduleName);

    public void ChangeLanguage(string language)
    {
        Guard.Against.NullOrWhiteSpace(language);
        DefaultLanguage = language;
    }

    public void ChangeCurrency(string currency)
    {
        Guard.Against.NullOrWhiteSpace(currency);
        DefaultCurrency = currency;
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
