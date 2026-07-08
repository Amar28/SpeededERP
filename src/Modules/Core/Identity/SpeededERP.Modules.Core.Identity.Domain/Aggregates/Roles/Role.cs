using Ardalis.GuardClauses;
using SpeededERP.Modules.Core.Identity.Domain.ValueObjects;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Identity.Domain.Aggregates.Roles;

/// <summary>
/// A named bundle of atomic permissions, scoped to a tenant. System roles
/// (<see cref="IsSystemRole"/>) ship with the platform and cannot be edited/deleted by
/// tenant administrators — only composed into custom roles.
/// </summary>
public sealed class Role : AggregateRoot<Guid>, IAuditable, IMustHaveTenant
{
    private readonly List<string> _permissions = [];

    public string Name { get; private set; } = null!;

    public string? Description { get; private set; }

    public bool IsSystemRole { get; private set; }

    public IReadOnlyCollection<string> Permissions => _permissions;

    public Guid TenantId { get; private set; }

    public DateTimeOffset CreatedOnUtc { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTimeOffset? LastModifiedOnUtc { get; private set; }

    public string? LastModifiedBy { get; private set; }

    private Role()
    {
    }

    private Role(Guid id, string name, string? description, bool isSystemRole)
        : base(id)
    {
        Name = name;
        Description = description;
        IsSystemRole = isSystemRole;
    }

    public static Result<Role> Create(string name, string? description, bool isSystemRole = false)
    {
        Guard.Against.NullOrWhiteSpace(name);

        return Result.Success(new Role(Guid.NewGuid(), name.Trim(), description, isSystemRole));
    }

    public Result Grant(PermissionCode permission)
    {
        if (IsSystemRole)
        {
            return Result.Failure(Error.Conflict("Role.SystemRoleImmutable", "Un rôle système ne peut pas être modifié."));
        }

        if (!_permissions.Contains(permission.Value))
        {
            _permissions.Add(permission.Value);
        }

        return Result.Success();
    }

    public Result Revoke(PermissionCode permission)
    {
        if (IsSystemRole)
        {
            return Result.Failure(Error.Conflict("Role.SystemRoleImmutable", "Un rôle système ne peut pas être modifié."));
        }

        _permissions.Remove(permission.Value);
        return Result.Success();
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
