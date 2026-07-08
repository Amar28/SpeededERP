namespace SpeededERP.BuildingBlocks.Application.Security;

/// <summary>
/// Declares the atomic <c>ressource:action</c> permission a command/query requires.
/// Enforced by <see cref="Behaviors.AuthorizationBehavior{TRequest,TResponse}"/> — the
/// second of the three RBAC verification levels described in section 18 (the first is
/// the ASP.NET Core policy at the endpoint, the third is UI masking on the frontend).
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class RequirePermissionAttribute(string permission) : Attribute
{
    public string Permission { get; } = permission;
}
