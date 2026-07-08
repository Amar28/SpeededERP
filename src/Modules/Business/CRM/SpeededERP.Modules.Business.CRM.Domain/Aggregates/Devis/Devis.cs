using Ardalis.GuardClauses;
using SpeededERP.Modules.Business.CRM.Domain.Events;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.CRM.Domain.Aggregates.Devis;

/// <summary>
/// The aggregate walked through in architecture sections 07 (hexagonal cut) and 09
/// (event flow, Fig. 03). Consistency boundary: a Devis and its LigneDevis are always
/// persisted together; <see cref="Montant"/> is never stored, always derived from the
/// lines to prevent the two from drifting apart.
/// </summary>
public sealed class Devis : AggregateRoot<Guid>, IAuditable, IMustHaveTenant
{
    private readonly List<LigneDevis> _lignes = [];

    public Guid ClientId { get; private set; }

    public StatutDevis Statut { get; private set; }

    public string Devise { get; private set; } = "EUR";

    public IReadOnlyCollection<LigneDevis> Lignes => _lignes;

    public Money Montant => _lignes
        .Select(l => l.MontantLigne)
        .Aggregate(Money.Zero(Devise), (total, ligne) => total.Add(ligne));

    public Guid TenantId { get; private set; }

    public DateTimeOffset CreatedOnUtc { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTimeOffset? LastModifiedOnUtc { get; private set; }

    public string? LastModifiedBy { get; private set; }

    private Devis()
    {
    }

    private Devis(Guid id, Guid clientId, string devise)
        : base(id)
    {
        ClientId = clientId;
        Devise = devise;
        Statut = StatutDevis.Brouillon;
    }

    public static Result<Devis> Creer(Guid clientId, string devise = "EUR")
    {
        Guard.Against.Default(clientId);

        return Result.Success(new Devis(Guid.NewGuid(), clientId, devise));
    }

    public Result AjouterLigne(string description, decimal quantite, Money prixUnitaire)
    {
        if (Statut != StatutDevis.Brouillon)
        {
            return Result.Failure(Error.Conflict("Devis.NonModifiable", "Seul un devis en brouillon peut être modifié."));
        }

        if (prixUnitaire.Currency != Devise)
        {
            return Result.Failure(Error.Validation("Devis.DeviseIncoherente", "La devise de la ligne ne correspond pas à celle du devis."));
        }

        var ligneResult = LigneDevis.Create(description, quantite, prixUnitaire);
        if (ligneResult.IsFailure)
        {
            return ligneResult;
        }

        _lignes.Add(ligneResult.Value);
        return Result.Success();
    }

    public Result Envoyer()
    {
        if (Statut != StatutDevis.Brouillon)
        {
            return Result.Failure(Error.Conflict("Devis.EtatInvalide", "Seul un devis en brouillon peut être envoyé."));
        }

        if (_lignes.Count == 0)
        {
            return Result.Failure(Error.Validation("Devis.Vide", "Un devis doit contenir au moins une ligne avant d'être envoyé."));
        }

        Statut = StatutDevis.Envoye;
        Raise(new DevisEnvoyeDomainEvent(Id, TenantId));
        return Result.Success();
    }

    /// <summary>Section 09, Fig. 03, step 1 — the domain event that ultimately triggers Comptabilité's invoice creation.</summary>
    public Result Accepter()
    {
        if (Statut != StatutDevis.Envoye)
        {
            return Result.Failure(Error.Conflict("Devis.EtatInvalide", "Seul un devis envoyé peut être accepté."));
        }

        Statut = StatutDevis.Accepte;
        Raise(new DevisAcceptedDomainEvent(Id, ClientId, TenantId, Montant.Amount, Devise));
        return Result.Success();
    }

    public Result Refuser()
    {
        if (Statut != StatutDevis.Envoye)
        {
            return Result.Failure(Error.Conflict("Devis.EtatInvalide", "Seul un devis envoyé peut être refusé."));
        }

        Statut = StatutDevis.Refuse;
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
