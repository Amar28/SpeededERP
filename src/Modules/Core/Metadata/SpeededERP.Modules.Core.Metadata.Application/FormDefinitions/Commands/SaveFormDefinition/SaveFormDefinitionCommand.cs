using SpeededERP.BuildingBlocks.Application.Messaging;
using SpeededERP.BuildingBlocks.Application.Security;

namespace SpeededERP.Modules.Core.Metadata.Application.FormDefinitions.Commands.SaveFormDefinition;

/// <summary>Creates a new FormDefinition, or a new version of an existing one when <see cref="FormDefinitionId"/> is set.</summary>
[RequirePermission("metadata:manage-forms")]
public sealed record SaveFormDefinitionCommand(
    Guid? FormDefinitionId,
    Guid EntityDefinitionId,
    string Name,
    string LayoutJson) : ICommand<Guid>;
