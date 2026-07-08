using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Identity.Application.Users;

/// <summary>
/// Outbound port to the technical credential store (ASP.NET Core Identity
/// <c>UserManager&lt;ApplicationUser&gt;</c> in Infrastructure). Kept separate from
/// <see cref="Domain.Repositories.IUserRepository"/>, which persists the business
/// profile aggregate — see the split explained on <c>Domain.Aggregates.Users.User</c>.
/// </summary>
public interface IUserCredentialStore
{
    /// <summary>Creates the credential record and returns its Id, shared with the business User aggregate.</summary>
    Task<Result<Guid>> CreateAsync(Guid userId, string email, string password, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default);
}
