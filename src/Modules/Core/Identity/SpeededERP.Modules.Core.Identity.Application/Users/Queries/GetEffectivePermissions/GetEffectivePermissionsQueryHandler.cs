using MediatR;
using SpeededERP.Modules.Core.Identity.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Identity.Application.Users.Queries.GetEffectivePermissions;

public sealed class GetEffectivePermissionsQueryHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository)
    : IRequestHandler<GetEffectivePermissionsQuery, Result<IReadOnlyCollection<string>>>
{
    public async Task<Result<IReadOnlyCollection<string>>> Handle(
        GetEffectivePermissionsQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken).ConfigureAwait(false);
        if (user is null)
        {
            return Result.Failure<IReadOnlyCollection<string>>(Error.NotFound("User.NotFound", "Utilisateur introuvable."));
        }

        var roles = await roleRepository.GetByIdsAsync(user.RoleIds, cancellationToken).ConfigureAwait(false);

        IReadOnlyCollection<string> permissions = roles
            .SelectMany(r => r.Permissions)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return Result.Success(permissions);
    }
}
