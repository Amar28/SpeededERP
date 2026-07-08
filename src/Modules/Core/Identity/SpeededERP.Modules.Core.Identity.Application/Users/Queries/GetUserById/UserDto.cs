namespace SpeededERP.Modules.Core.Identity.Application.Users.Queries.GetUserById;

public sealed record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Status,
    IReadOnlyCollection<Guid> RoleIds);
