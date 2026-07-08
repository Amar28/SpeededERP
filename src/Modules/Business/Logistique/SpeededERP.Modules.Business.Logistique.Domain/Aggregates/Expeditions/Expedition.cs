using Ardalis.GuardClauses;
using SpeededERP.Modules.Business.Logistique.Domain.Events;
using SpeededERP.Modules.Business.Logistique.Domain.ValueObjects;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.Logistique.Domain.Aggregates.Expeditions;

/// <summary>
/// The Logistique module's reference aggregate (architecture section 07). Walks the
/// same state-machine shape as CRM's Devis: Preparee -&gt; EnTransit -&gt; Livree, with a
/// side branch to Retournee.
/// </summary>
public sealed class Expedition : AggregateRoot<Guid>, IAuditable, IMustHaveTenant
{
    public string NumeroSuivi { get; private set; } = null!;

    public string TransporteurNom { get; private set; } = null!;

    public AdresseLivraison AdresseLivraison { get; private set; } = null!;

    public StatutExpedition Statut { get; private set; }

    public Guid TenantId { get; private set; }

    public DateTimeOffset CreatedOnUtc { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTimeOffset? LastModifiedOnUtc { get; private set; }

    public string? LastModifiedBy { get; private set; }

    private Expedition()
    {
    }

    private Expedition(Guid id, string numeroSuivi, string transporteurNom, AdresseLivraison adresseLivraison)
        : base(id)
    {
        NumeroSuivi = numeroSuivi;
        TransporteurNom = transporteurNom;
        AdresseLivraison = adresseLivraison;
        Statut = StatutExpedition.Preparee;
    }

    public static Result<Expedition> Preparer(string numeroSuivi, string transporteurNom, AdresseLivraison adresseLivraison)
    {
        Guard.Against.NullOrWhiteSpace(numeroSuivi);
        Guard.Against.NullOrWhiteSpace(transporteurNom);

        return Result.Success(new Expedition(Guid.NewGuid(), numeroSuivi.Trim(), transporteurNom.Trim(), adresseLivraison));
    }

    public Result DemarrerTransit()
    {
        if (Statut != StatutExpedition.Preparee)
        {
            return Result.Failure(Error.Conflict("Expedition.EtatInvalide", "Seule une expédition préparée peut démarrer son transit."));
        }

        Statut = StatutExpedition.EnTransit;
        return Result.Success();
    }

    public Result ConfirmerLivraison()
    {
        if (Statut != StatutExpedition.EnTransit)
        {
            return Result.Failure(Error.Conflict("Expedition.EtatInvalide", "Seule une expédition en transit peut être confirmée comme livrée."));
        }

        Statut = StatutExpedition.Livree;
        Raise(new ExpeditionLivreeDomainEvent(Id, TenantId));
        return Result.Success();
    }

    public Result Retourner()
    {
        if (Statut is not (StatutExpedition.EnTransit or StatutExpedition.Livree))
        {
            return Result.Failure(Error.Conflict("Expedition.EtatInvalide", "Seule une expédition en transit ou livrée peut être retournée."));
        }

        Statut = StatutExpedition.Retournee;
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
