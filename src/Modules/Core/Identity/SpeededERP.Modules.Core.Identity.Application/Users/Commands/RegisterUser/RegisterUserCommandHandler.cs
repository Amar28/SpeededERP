using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Application.Outbox;
using SpeededERP.BuildingBlocks.Application.Security;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Identity.Application.IntegrationEvents;
using SpeededERP.Modules.Core.Identity.Domain.Aggregates.Roles;
using SpeededERP.Modules.Core.Identity.Domain.Aggregates.Users;
using SpeededERP.Modules.Core.Identity.Domain.Repositories;
using SpeededERP.Modules.Core.Identity.Domain.ValueObjects;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Identity.Application.Users.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUserCredentialStore credentialStore,
    [FromKeyedServices("identity")] IUnitOfWork unitOfWork,
    [FromKeyedServices("identity")] IOutboxWriter outboxWriter,
    ITenantContext tenantContext)
    : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (tenantContext.TenantId == Guid.Empty)
        {
            return Result.Failure<Guid>(Error.Validation(
                "User.TenantNotResolved",
                "Impossible de déterminer l'organisation cible — en-tête X-Tenant-Slug manquant ou invalide."));
        }

        var emailResult = EmailAddress.Create(request.Email);
        if (emailResult.IsFailure)
        {
            return Result.Failure<Guid>(emailResult.Error);
        }

        if (await userRepository.ExistsWithEmailAsync(emailResult.Value, tenantContext.TenantId, cancellationToken).ConfigureAwait(false))
        {
            return Result.Failure<Guid>(Error.Conflict("User.EmailAlreadyUsed", "Cette adresse e-mail est déjà utilisée."));
        }

        // No other way exists yet to obtain an authenticated caller for a brand-new
        // tenant — creating and assigning a role both require RequireAuthorization()
        // endpoints. The tenant's first registrant becomes its administrator
        // atomically, in the same unit of work, rather than through those endpoints.
        var isFirstUserOfTenant = !await userRepository.AnyForTenantAsync(tenantContext.TenantId, cancellationToken).ConfigureAwait(false);

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

        var user = userResult.Value;

        if (isFirstUserOfTenant)
        {
            var roleResult = Role.Create("Admin", "Administrateur — accès complet", isSystemRole: true);
            var role = roleResult.Value;
            role.AssignTenant(tenantContext.TenantId);
            roleRepository.Add(role);

            user.AssignRole(role.Id);
            user.Activate();
        }

        userRepository.Add(user);

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
