using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Application.Outbox;
using SpeededERP.BuildingBlocks.Application.Security;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Identity.Application.IntegrationEvents;
using SpeededERP.Modules.Core.Identity.Domain.Aggregates.Users;
using SpeededERP.Modules.Core.Identity.Domain.Repositories;
using SpeededERP.Modules.Core.Identity.Domain.ValueObjects;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Identity.Application.Users.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IUserCredentialStore credentialStore,
    [FromKeyedServices("identity")] IUnitOfWork unitOfWork,
    [FromKeyedServices("identity")] IOutboxWriter outboxWriter,
    ITenantContext tenantContext)
    : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var emailResult = EmailAddress.Create(request.Email);
        if (emailResult.IsFailure)
        {
            return Result.Failure<Guid>(emailResult.Error);
        }

        if (await userRepository.ExistsWithEmailAsync(emailResult.Value, tenantContext.TenantId, cancellationToken).ConfigureAwait(false))
        {
            return Result.Failure<Guid>(Error.Conflict("User.EmailAlreadyUsed", "Cette adresse e-mail est déjà utilisée."));
        }

        var userId = Guid.NewGuid();

        var credentialResult = await credentialStore
            .CreateAsync(userId, emailResult.Value.Value, request.InitialPassword, cancellationToken)
            .ConfigureAwait(false);

        if (credentialResult.IsFailure)
        {
            return Result.Failure<Guid>(credentialResult.Error);
        }

        var userResult = User.Register(userId, emailResult.Value, request.FirstName, request.LastName, tenantContext.TenantId);
        if (userResult.IsFailure)
        {
            await credentialStore.DeleteAsync(userId, cancellationToken).ConfigureAwait(false);
            return Result.Failure<Guid>(userResult.Error);
        }

        userRepository.Add(userResult.Value);

        await outboxWriter.EnqueueAsync(
            new UserRegisteredIntegrationEvent
            {
                UserId = userId,
                Email = emailResult.Value.Value,
                FullName = $"{request.FirstName} {request.LastName}",
                TenantId = tenantContext.TenantId,
            },
            cancellationToken).ConfigureAwait(false);

        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(userId);
    }
}
