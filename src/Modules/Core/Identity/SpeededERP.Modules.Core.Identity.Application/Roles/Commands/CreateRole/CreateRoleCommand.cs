using SpeededERP.BuildingBlocks.Application.Messaging;
using SpeededERP.BuildingBlocks.Application.Security;

namespace SpeededERP.Modules.Core.Identity.Application.Roles.Commands.CreateRole;

[RequirePermission("identity:roles:manage")]
public sealed record CreateRoleCommand(string Name, string? Description, IReadOnlyCollection<string> Permissions) : ICommand<Guid>;
