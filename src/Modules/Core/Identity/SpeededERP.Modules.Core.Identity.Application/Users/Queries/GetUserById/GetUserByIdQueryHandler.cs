using MediatR;
using SpeededERP.Modules.Core.Identity.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Identity.Application.Users.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken).ConfigureAwait(false);
        if (user is null)
        {
            return Result.Failure<UserDto>(Error.NotFound("User.NotFound", "Utilisateur introuvable."));
        }

        var dto = new UserDto(user.Id, user.Email.Value, user.FirstName, user.LastName, user.Status.ToString(), user.RoleIds);
        return Result.Success(dto);
    }
}
