namespace SpeededERP.Modules.Core.Identity.Presentation.Contracts;

public sealed record CreateRoleRequest(string Name, string? Description, IReadOnlyCollection<string> Permissions);
