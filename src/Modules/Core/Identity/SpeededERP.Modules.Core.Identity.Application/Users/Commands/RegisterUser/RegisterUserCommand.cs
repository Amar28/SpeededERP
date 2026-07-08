using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Core.Identity.Application.Users.Commands.RegisterUser;

/// <summary>
/// Registers the business profile (Core.Identity.Domain.User) and, transactionally on
/// the Infrastructure side, the ASP.NET Core Identity credential record — see
/// <c>RegisterUserCommandHandler</c> and <c>IUserCredentialStore</c>.
/// </summary>
public sealed record RegisterUserCommand(
    string Email,
    string FirstName,
    string LastName,
    string InitialPassword) : ICommand<Guid>;
