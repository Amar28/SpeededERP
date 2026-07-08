using SpeededERP.BuildingBlocks.Application.Messaging;
using SpeededERP.BuildingBlocks.Application.Security;

namespace SpeededERP.Modules.Core.Identity.Application.Users.Commands.AssignRole;

[RequirePermission("identity:users:manage-roles")]
public sealed record AssignRoleCommand(Guid UserId, Guid RoleId) : ICommand;
