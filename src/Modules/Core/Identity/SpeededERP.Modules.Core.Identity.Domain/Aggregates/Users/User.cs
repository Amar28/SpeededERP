using Ardalis.GuardClauses;
using SpeededERP.Modules.Core.Identity.Domain.Events;
using SpeededERP.Modules.Core.Identity.Domain.ValueObjects;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Identity.Domain.Aggregates.Users;

public enum UserStatus
{
    PendingActivation,
    Active,
    Deactivated,
}

/// <summary>
/// Business-side user profile: identity, tenant membership, role assignments and
/// status. Authentication credentials (password hash, MFA, lockout) are a distinct
/// technical concern owned by ASP.NET Core Identity's <c>ApplicationUser</c> in the
/// Infrastructure layer, correlated 1:1 by <see cref="Entity{TId}.Id"/> — the Domain
/// layer never touches credential material, only business identity and authorization.
/// </summary>
public sealed class User : AggregateRoot<Guid>, IAuditable, IMustHaveTenant
{
    private readonly List<Guid> _roleIds = [];

    public EmailAddress Email { get; private set; } = null!;

    public string FirstName { get; private set; } = null!;

    public string LastName { get; private set; } = null!;

    public UserStatus Status { get; private set; }

    public Guid TenantId { get; private set; }

    public IReadOnlyCollection<Guid> RoleIds => _roleIds;

    public DateTimeOffset CreatedOnUtc { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTimeOffset? LastModifiedOnUtc { get; private set; }

    public string? LastModifiedBy { get; private set; }

    private User()
    {
    }

    private User(Guid id, EmailAddress email, string firstName, string lastName)
        : base(id)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Status = UserStatus.PendingActivation;
    }

    public static Result<User> Register(Guid id, EmailAddress email, string firstName, string lastName, Guid tenantId)
    {
        Guard.Against.NullOrWhiteSpace(firstName);
        Guard.Against.NullOrWhiteSpace(lastName);
        Guard.Against.Default(tenantId);

        var user = new User(id, email, firstName.Trim(), lastName.Trim())
        {
            TenantId = tenantId,
        };

        user.Raise(new UserRegisteredDomainEvent(user.Id, tenantId, email.Value));

        return Result.Success(user);
    }

    public Result Activate()
    {
        if (Status == UserStatus.Active)
        {
            return Result.Failure(Error.Conflict("User.AlreadyActive", "L'utilisateur est déjà actif."));
        }

        Status = UserStatus.Active;
        Raise(new UserActivatedDomainEvent(Id, TenantId));
        return Result.Success();
    }

    public Result Deactivate()
    {
        if (Status == UserStatus.Deactivated)
        {
            return Result.Failure(Error.Conflict("User.AlreadyDeactivated", "L'utilisateur est déjà désactivé."));
        }

        Status = UserStatus.Deactivated;
        Raise(new UserDeactivatedDomainEvent(Id, TenantId));
        return Result.Success();
    }

    public Result AssignRole(Guid roleId)
    {
        if (Status == UserStatus.Deactivated)
        {
            return Result.Failure(Error.Conflict("User.Deactivated", "Impossible d'assigner un rôle à un utilisateur désactivé."));
        }

        if (_roleIds.Contains(roleId))
        {
            return Result.Failure(Error.Conflict("User.RoleAlreadyAssigned", "Ce rôle est déjà assigné à l'utilisateur."));
        }

        _roleIds.Add(roleId);
        Raise(new RoleAssignedToUserDomainEvent(Id, roleId, TenantId));
        return Result.Success();
    }

    public void RevokeRole(Guid roleId) => _roleIds.Remove(roleId);

    public void Rename(string firstName, string lastName)
    {
        Guard.Against.NullOrWhiteSpace(firstName);
        Guard.Against.NullOrWhiteSpace(lastName);

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    public void SetCreated(DateTimeOffset onUtc, string? by)
    {
        CreatedOnUtc = onUtc;
        CreatedBy = by;
    }

    public void SetModified(DateTimeOffset onUtc, string? by)
    {
        LastModifiedOnUtc = onUtc;
        LastModifiedBy = by;
    }

    public void AssignTenant(Guid tenantId) => TenantId = tenantId;
}
