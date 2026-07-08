using MassTransit;
using Microsoft.Extensions.Hosting;
using SpeededERP.BuildingBlocks.Messaging.Contracts;
using SpeededERP.Modules.Business.CRM.Domain.Aggregates.Clients;

namespace SpeededERP.Modules.Business.CRM.Infrastructure.Startup;

/// <summary>
/// Declares the entities CRM owns to Core.Metadata at process startup — the producer
/// side of architecture section 10's no-code engine. Idempotent: Core.Metadata upserts
/// by a deterministic Id derived from (module, entity), so republishing on every
/// restart is safe and requires no coordination between modules.
/// </summary>
public sealed class EntityDefinitionSeeder(IPublishEndpoint publishEndpoint) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(
            new EntityDefinitionDeclaredIntegrationEvent
            {
                ModuleName = "CRM",
                EntityName = nameof(Client),
                DisplayNameKey = "crm.entities.client",
                SupportsCustomFields = true,
            },
            cancellationToken).ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
