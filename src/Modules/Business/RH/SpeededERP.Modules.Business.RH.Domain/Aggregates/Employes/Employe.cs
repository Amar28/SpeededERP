using Ardalis.GuardClauses;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.RH.Domain.Aggregates.Employes;

/// <summary>
/// Employee aggregate. <see cref="Email"/> is kept a plain validated string rather than a
/// dedicated value object to keep this module's scope modest — see the module README/task
/// scope notes; promote to a value object if RH grows richer contact-info requirements.
/// </summary>
public sealed class Employe : AggregateRoot<Guid>, IAuditable, IMustHaveTenant
{
    public string Nom { get; private set; } = null!;

    public string Prenom { get; private set; } = null!;

    public string Email { get; private set; } = null!;

    public string Poste { get; private set; } = null!;

    public DateTimeOffset DateEmbaucheUtc { get; private set; }

    public StatutEmploye Statut { get; private set; }

    public Guid TenantId { get; private set; }

    public DateTimeOffset CreatedOnUtc { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTimeOffset? LastModifiedOnUtc { get; private set; }

    public string? LastModifiedBy { get; private set; }

    private Employe()
    {
    }

    private Employe(Guid id, string nom, string prenom, string email, string poste, DateTimeOffset dateEmbaucheUtc)
        : base(id)
    {
        Nom = nom;
        Prenom = prenom;
        Email = email;
        Poste = poste;
        DateEmbaucheUtc = dateEmbaucheUtc;
        Statut = StatutEmploye.Actif;
    }

    public static Result<Employe> Embaucher(string nom, string prenom, string email, string poste, DateTimeOffset dateEmbaucheUtc)
    {
        Guard.Against.NullOrWhiteSpace(nom);
        Guard.Against.NullOrWhiteSpace(prenom);
        Guard.Against.NullOrWhiteSpace(email);
        Guard.Against.NullOrWhiteSpace(poste);

        if (!email.Contains('@', StringComparison.Ordinal))
        {
            return Result.Failure<Employe>(Error.Validation("Employe.EmailInvalide", "L'adresse e-mail n'est pas valide."));
        }

        return Result.Success(new Employe(Guid.NewGuid(), nom.Trim(), prenom.Trim(), email.Trim(), poste.Trim(), dateEmbaucheUtc));
    }

    public Result MettreEnConge()
    {
        if (Statut != StatutEmploye.Actif)
        {
            return Result.Failure(Error.Conflict("Employe.EtatInvalide", "Seul un employé actif peut être mis en congé."));
        }

        Statut = StatutEmploye.EnConge;
        return Result.Success();
    }

    public Result ReprendreActivite()
    {
        if (Statut != StatutEmploye.EnConge)
        {
            return Result.Failure(Error.Conflict("Employe.EtatInvalide", "Seul un employé en congé peut reprendre son activité."));
        }

        Statut = StatutEmploye.Actif;
        return Result.Success();
    }

    public Result Depart()
    {
        if (Statut == StatutEmploye.Parti)
        {
            return Result.Failure(Error.Conflict("Employe.EtatInvalide", "Cet employé a déjà quitté l'entreprise."));
        }

        Statut = StatutEmploye.Parti;
        return Result.Success();
    }

    public void AssignTenant(Guid tenantId) => TenantId = tenantId;

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
