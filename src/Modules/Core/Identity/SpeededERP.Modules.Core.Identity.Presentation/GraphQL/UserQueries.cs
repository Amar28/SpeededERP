using HotChocolate;
using HotChocolate.Types;
using MediatR;
using SpeededERP.Modules.Core.Identity.Application.Users.Queries.GetUserById;

namespace SpeededERP.Modules.Core.Identity.Presentation.GraphQL;

/// <summary>
/// GraphQL adapter over the same Application layer as <c>UserEndpoints</c> — no
/// business logic duplicated between REST and GraphQL (architecture section 19).
/// Registered into the shared schema by <c>ModuleRegistration</c> via
/// <c>[ExtendObjectType(typeof(Query))]</c>.
/// </summary>
[ExtendObjectType(OperationTypeNames.Query)]
public sealed class UserQueries
{
    public async Task<UserDto?> GetUserById(Guid userId, [Service] ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetUserByIdQuery(userId), cancellationToken).ConfigureAwait(false);
        return result.IsSuccess ? result.Value : null;
    }
}
