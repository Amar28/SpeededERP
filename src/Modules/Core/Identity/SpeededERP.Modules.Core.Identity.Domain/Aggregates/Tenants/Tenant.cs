using Ardalis.GuardClauses;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Identity.Domain.Aggregates.Tenants;

public enum TenantIsolationMode
{
    Shared,
    Dedicated,
    OnPremiseSingleTenant,
}

public enum TenantStatus
{
    Trial,
    Active,
    Suspended,
    Archived,
}

/// <summary>
/// A subscribed company (architecture section 17). Owns the isolation-mode decision
/// made at subscription time and the set of activated modules (section 05/16) — never
/// a per-request concern.
/// </summary>
public sealed class Tenant : AggregateRoot<Guid>, IAuditable
{
    private readonly List<string> _activatedModules = [];

    public string CompanyName { get; private set; } = null!;

    public string Slug { get; private set; } = null!;

    public TenantIsolationMode IsolationMode { get; private set; }

    public TenantStatus Status { get; private set; }

    public string DefaultLanguage { get; private set; } = "fr";

    public string DefaultCurrency { get; private set; } = "EUR";

    public IReadOnlyCollection<string> ActivatedModules => _activatedModules;

    public DateTimeOffset CreatedOnUtc { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTimeOffset? LastModifiedOnUtc { get; private set; }

    public string? LastModifiedBy { get; private set; }

    private Tenant()
    {
    }

    private Tenant(Guid id, string companyName, string slug, TenantIsolationMode isolationMode)
        : base(id)
    {
        CompanyName = companyName;
        Slug = slug;
        IsolationMode = isolationMode;
        Status = TenantStatus.Trial;
    }

    public static Result<Tenant> Provision(string companyName, string slug, TenantIsolationMode isolationMode)
    {
        Guard.Against.NullOrWhiteSpace(companyName);
        Guard.Against.NullOrWhiteSpace(slug);

        var tenant = new Tenant(Guid.NewGuid(), companyName.Trim(), slug.Trim().ToLowerInvariant(), isolationMode);
        tenant._activatedModules.Add("Core.Identity");
        tenant._activatedModules.Add("Core.Metadata");
        tenant._activatedModules.Add("Core.Config");

        return Result.Success(tenant);
    }

    public Result ActivateModule(string moduleName)
    {
        if (Status is TenantStatus.Suspended or TenantStatus.Archived)
        {
            return Result.Failure(Error.Conflict("Tenant.Inactive", "Impossible d'activer un module pour un tenant inactif."));
        }

        if (!_activatedModules.Contains(moduleName))
        {
            _activatedModules.Add(moduleName);
        }

        return Result.Success();
    }

    public void DeactivateModule(string moduleName) => _activatedModules.Remove(moduleName);

    public Result Activate()
    {
        if (Status == TenantStatus.Archived)
        {
            return Result.Failure(Error.Conflict("Tenant.Archived", "Un tenant archivé ne peut pas être réactivé."));
        }

        Status = TenantStatus.Active;
        return Result.Success();
    }

    public void Suspend() => Status = TenantStatus.Suspended;

    public void Archive() => Status = TenantStatus.Archived;

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
}
