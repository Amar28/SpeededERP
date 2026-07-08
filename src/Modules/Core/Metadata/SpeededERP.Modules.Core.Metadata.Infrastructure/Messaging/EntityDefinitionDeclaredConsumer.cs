using MassTransit;
using MediatR;
using SpeededERP.BuildingBlocks.Messaging.Contracts;
using SpeededERP.Modules.Core.Metadata.Application.EntityDefinitions.Commands.UpsertEntityDefinition;

namespace SpeededERP.Modules.Core.Metadata.Infrastructure.Messaging;

/// <summary>
/// The only place Core.Metadata knows about other modules' existence — by contract,
/// not by assembly reference (architecture section 09).
/// </summary>
public sealed class EntityDefinitionDeclaredConsumer(ISender sender) : IConsumer<EntityDefinitionDeclaredIntegrationEvent>
{
    public async Task Consume(ConsumeContext<EntityDefinitionDeclaredIntegrationEvent> context)
    {
        var message = context.Message;

        await sender.Send(
            new UpsertEntityDefinitionCommand(message.ModuleName, message.EntityName, message.DisplayNameKey, message.SupportsCustomFields),
            context.CancellationToken).ConfigureAwait(false);
    }
}
