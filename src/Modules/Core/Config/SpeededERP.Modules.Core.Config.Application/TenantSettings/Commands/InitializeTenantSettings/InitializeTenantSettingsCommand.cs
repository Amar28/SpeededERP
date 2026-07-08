using SpeededERP.BuildingBlocks.Application.Messaging;
using SpeededERP.BuildingBlocks.Application.Security;

namespace SpeededERP.Modules.Core.Config.Application.TenantSettings.Commands.InitializeTenantSettings;

[RequirePermission("config:manage")]
public sealed record InitializeTenantSettingsCommand(
    Guid TenantId,
    string DefaultLanguage,
    string DefaultCurrency,
    string TimeZoneId) : ICommand<Guid>;
