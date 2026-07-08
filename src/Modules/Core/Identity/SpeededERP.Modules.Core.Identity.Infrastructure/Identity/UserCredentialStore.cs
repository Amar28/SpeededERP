using Microsoft.AspNetCore.Identity;
using SpeededERP.BuildingBlocks.Application.Security;
using SpeededERP.Modules.Core.Identity.Application.Users;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Identity.Infrastructure.Identity;

public sealed class UserCredentialStore(UserManager<ApplicationUser> userManager, ITenantContext tenantContext)
    : IUserCredentialStore
{
    public async Task<Result<Guid>> CreateAsync(Guid userId, string email, string password, CancellationToken cancellationToken = default)
    {
        var applicationUser = new ApplicationUser
        {
            Id = userId,
            UserName = email,
            Email = email,
            TenantId = tenantContext.TenantId,
        };

        var result = await userManager.CreateAsync(applicationUser, password).ConfigureAwait(false);

        if (!result.Succeeded)
        {
            var message = string.Join(" | ", result.Errors.Select(e => e.Description));
            return Result.Failure<Guid>(Error.Validation("User.CredentialCreationFailed", message));
        }

        return Result.Success(userId);
    }

    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var applicationUser = await userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);
        if (applicationUser is not null)
        {
            await userManager.DeleteAsync(applicationUser).ConfigureAwait(false);
        }
    }
}
