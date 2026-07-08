using SpeededERP.BuildingBlocks.Application.Messaging;
using SpeededERP.Modules.Core.Identity.Domain.Aggregates.Tenants;

namespace SpeededERP.Modules.Core.Identity.Application.Tenants.Commands.ProvisionTenant;

public sealed record ProvisionTenantCommand(string CompanyName, string Slug, TenantIsolationMode IsolationMode) : ICommand<Guid>;
