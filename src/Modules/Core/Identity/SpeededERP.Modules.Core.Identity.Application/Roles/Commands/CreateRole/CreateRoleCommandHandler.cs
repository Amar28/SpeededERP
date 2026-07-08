using MediatR;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Identity.Domain.Aggregates.Roles;
using SpeededERP.Modules.Core.Identity.Domain.Repositories;
using SpeededERP.Modules.Core.Identity.Domain.ValueObjects;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Identity.Application.Roles.Commands.CreateRole;

public sealed class CreateRoleCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateRoleCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var roleResult = Role.Create(request.Name, request.Description);
        if (roleResult.IsFailure)
        {
            return Result.Failure<Guid>(roleResult.Error);
        }

        var role = roleResult.Value;

        foreach (var permissionValue in request.Permissions)
        {
            var permissionResult = PermissionCode.Create(permissionValue);
            if (permissionResult.IsFailure)
            {
                return Result.Failure<Guid>(permissionResult.Error);
            }

            role.Grant(permissionResult.Value);
        }

        roleRepository.Add(role);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(role.Id);
    }
}
