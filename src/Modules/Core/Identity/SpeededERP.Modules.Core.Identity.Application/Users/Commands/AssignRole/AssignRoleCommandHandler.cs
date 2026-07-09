using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Identity.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Identity.Application.Users.Commands.AssignRole;

public sealed class AssignRoleCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    [FromKeyedServices("identity")] IUnitOfWork unitOfWork)
    : IRequestHandler<AssignRoleCommand, Result>
{
    public async Task<Result> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken).ConfigureAwait(false);
        if (user is null)
        {
            return Result.Failure(Error.NotFound("User.NotFound", "Utilisateur introuvable."));
        }

        var role = await roleRepository.GetByIdAsync(request.RoleId, cancellationToken).ConfigureAwait(false);
        if (role is null)
        {
            return Result.Failure(Error.NotFound("Role.NotFound", "Rôle introuvable."));
        }

        var result = user.AssignRole(role.Id);
        if (result.IsFailure)
        {
            return result;
        }

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success();
    }
}
