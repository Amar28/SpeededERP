using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Core.Config.Application.TenantSettings.Queries.GetTenantSettings;

/// <summary>Tenant config is read on nearly every request (architecture section 25) — cached for 30 minutes.</summary>
public sealed record GetTenantSettingsQuery(Guid TenantId) : IQuery<TenantSettingsDto>, ICacheableQuery
{
    public string CacheKey => $"config:tenant-settings:{TenantId}";

    public TimeSpan? Expiration => TimeSpan.FromMinutes(30);
}
