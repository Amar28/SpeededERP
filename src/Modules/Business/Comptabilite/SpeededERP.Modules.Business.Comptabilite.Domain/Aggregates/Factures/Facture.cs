using Ardalis.GuardClauses;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.Comptabilite.Domain.Aggregates.Factures;

/// <summary>
/// Invoice aggregate (architecture section 09, Fig. 03 — conceptually created from CRM's
/// DevisAccepted event once a Devis is accepted). This pass implements the aggregate and
/// its CQRS surface only; the actual cross-module integration-event consumer that would
/// auto-create invoices is future work — see Infrastructure.DependencyInjection.
/// </summary>
public sealed class Facture : AggregateRoot<Guid>, IAuditable, IMustHaveTenant
{
    public Guid ClientId { get; private set; }

    public Money Montant { get; private set; } = null!;

    public StatutFacture Statut { get; private set; }

    public DateTimeOffset? DateEmissionUtc { get; private set; }

    public string? ReferenceExterne { get; private set; }

    public Guid TenantId { get; private set; }

    public DateTimeOffset CreatedOnUtc { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTimeOffset? LastModifiedOnUtc { get; private set; }

    public string? LastModifiedBy { get; private set; }

    private Facture()
    {
    }

    private Facture(Guid id, Guid clientId, Money montant, string? referenceExterne)
        : base(id)
    {
        ClientId = clientId;
        Montant = montant;
        ReferenceExterne = referenceExterne;
        Statut = StatutFacture.Brouillon;
    }

    public static Result<Facture> Creer(Guid clientId, Money montant, string? referenceExterne = null)
    {
        Guard.Against.Default(clientId);
        Guard.Against.Null(montant);

        if (montant.Amount < 0)
        {
            return Result.Failure<Facture>(Error.Validation("Facture.MontantNegatif", "Le montant de la facture ne peut pas être négatif."));
        }

        return Result.Success(new Facture(Guid.NewGuid(), clientId, montant, referenceExterne));
    }

    public Result Emettre()
    {
        if (Statut != StatutFacture.Brouillon)
        {
            return Result.Failure(Error.Conflict("Facture.EtatInvalide", "Seule une facture en brouillon peut être émise."));
        }

        Statut = StatutFacture.Emise;
        DateEmissionUtc = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result MarquerPayee()
    {
        if (Statut != StatutFacture.Emise)
        {
            return Result.Failure(Error.Conflict("Facture.EtatInvalide", "Seule une facture émise peut être marquée payée."));
        }

        Statut = StatutFacture.Payee;
        return Result.Success();
    }

    public Result Annuler()
    {
        if (Statut is not (StatutFacture.Brouillon or StatutFacture.Emise))
        {
            return Result.Failure(Error.Conflict("Facture.EtatInvalide", "Seule une facture en brouillon ou émise peut être annulée."));
        }

        Statut = StatutFacture.Annulee;
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
