using SpeededERP.Modules.Core.Identity.Domain.Aggregates.Tenants;

namespace SpeededERP.Modules.Core.Identity.Presentation.Contracts;

public sealed record ProvisionTenantRequest(string CompanyName, string Slug, TenantIsolationMode IsolationMode);
