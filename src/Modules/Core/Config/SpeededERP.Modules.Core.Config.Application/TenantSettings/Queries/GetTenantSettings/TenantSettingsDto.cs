namespace SpeededERP.Modules.Core.Config.Application.TenantSettings.Queries.GetTenantSettings;

public sealed record TenantSettingsDto(
    Guid TenantId,
    string DefaultLanguage,
    string DefaultCurrency,
    string TimeZoneId,
    IReadOnlyCollection<string> ActivatedModules);
