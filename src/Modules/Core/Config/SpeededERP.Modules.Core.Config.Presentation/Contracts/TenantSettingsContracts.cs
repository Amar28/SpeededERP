namespace SpeededERP.Modules.Core.Config.Presentation.Contracts;

public sealed record InitializeTenantSettingsRequest(Guid TenantId, string DefaultLanguage, string DefaultCurrency, string TimeZoneId);
