using SpeededERP.BuildingBlocks.Messaging;

namespace SpeededERP.Modules.Core.Identity.Application.IntegrationEvents;

/// <summary>Consumed by every module that seeds tenant-scoped defaults on onboarding (Core.Config, Core.Metadata...).</summary>
public sealed record TenantProvisionedIntegrationEvent : IntegrationEvent
{
    public required string CompanyName { get; init; }

    public required string Slug { get; init; }
}
