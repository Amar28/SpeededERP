using Ardalis.GuardClauses;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.Stock.Domain.Aggregates.Produits;

/// <summary>
/// The Stock module's reference aggregate (architecture section 07): a Produit tracks
/// its own quantity in a single warehouse-agnostic counter. Consistency boundary: the
/// stock level is only ever mutated through <see cref="Reapprovisionner"/> and
/// <see cref="Retirer"/> so it can never go negative.
/// </summary>
public sealed class Produit : AggregateRoot<Guid>, IAuditable, IMustHaveTenant
{
    public string Reference { get; private set; } = null!;

    public string Nom { get; private set; } = null!;

    public int QuantiteEnStock { get; private set; }

    public int SeuilAlerte { get; private set; }

    public Money PrixUnitaire { get; private set; } = null!;

    public bool EstEnAlerte => QuantiteEnStock <= SeuilAlerte;

    public Guid TenantId { get; private set; }

    public DateTimeOffset CreatedOnUtc { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTimeOffset? LastModifiedOnUtc { get; private set; }

    public string? LastModifiedBy { get; private set; }

    private Produit()
    {
    }

    private Produit(Guid id, string reference, string nom, Money prixUnitaire, int seuilAlerte)
        : base(id)
    {
        Reference = reference;
        Nom = nom;
        PrixUnitaire = prixUnitaire;
        SeuilAlerte = seuilAlerte;
        QuantiteEnStock = 0;
    }

    public static Result<Produit> Creer(string reference, string nom, Money prixUnitaire, int seuilAlerte)
    {
        Guard.Against.NullOrWhiteSpace(reference);
        Guard.Against.NullOrWhiteSpace(nom);

        if (seuilAlerte < 0)
        {
            return Result.Failure<Produit>(Error.Validation("Produit.SeuilAlerteInvalide", "Le seuil d'alerte ne peut pas être négatif."));
        }

        if (prixUnitaire.Amount < 0)
        {
            return Result.Failure<Produit>(Error.Validation("Produit.PrixNegatif", "Le prix unitaire ne peut pas être négatif."));
        }

        return Result.Success(new Produit(Guid.NewGuid(), reference.Trim(), nom.Trim(), prixUnitaire, seuilAlerte));
    }

    public Result Reapprovisionner(int quantite)
    {
        if (quantite <= 0)
        {
            return Result.Failure(Error.Validation("Produit.QuantiteInvalide", "La quantité à réapprovisionner doit être strictement positive."));
        }

        QuantiteEnStock += quantite;
        return Result.Success();
    }

    public Result Retirer(int quantite)
    {
        if (quantite <= 0)
        {
            return Result.Failure(Error.Validation("Produit.QuantiteInvalide", "La quantité à retirer doit être strictement positive."));
        }

        if (quantite > QuantiteEnStock)
        {
            return Result.Failure(Error.Validation("Stock.Insuffisant", "La quantité en stock est insuffisante pour ce retrait."));
        }

        QuantiteEnStock -= quantite;
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
