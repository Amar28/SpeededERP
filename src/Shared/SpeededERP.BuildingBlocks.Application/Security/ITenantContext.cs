namespace SpeededERP.BuildingBlocks.Application.Security;

/// <summary>
/// Resolves the current tenant (JWT claim or sub-domain, section 17) and propagates it
/// to the DbContext global query filter and to outgoing MassTransit message headers.
/// </summary>
public interface ITenantContext
{
    Guid TenantId { get; }

    /// <summary>Shared database + Row-Level Security vs. a dedicated database (section 17).</summary>
    TenantIsolationMode IsolationMode { get; }
}

public enum TenantIsolationMode
{
    Shared,
    Dedicated,
    OnPremiseSingleTenant,
}
