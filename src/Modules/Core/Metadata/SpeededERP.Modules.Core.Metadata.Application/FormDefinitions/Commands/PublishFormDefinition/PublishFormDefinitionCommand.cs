using SpeededERP.BuildingBlocks.Application.Messaging;
using SpeededERP.BuildingBlocks.Application.Security;

namespace SpeededERP.Modules.Core.Metadata.Application.FormDefinitions.Commands.PublishFormDefinition;

[RequirePermission("metadata:manage-forms")]
public sealed record PublishFormDefinitionCommand(Guid FormDefinitionId) : ICommand;
