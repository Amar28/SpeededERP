using Ardalis.GuardClauses;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.CRM.Domain.Aggregates.Devis;

/// <summary>Entity local to the Devis aggregate — never accessed or persisted outside it.</summary>
public sealed class LigneDevis : Entity<Guid>
{
    public string Description { get; private set; } = null!;

    public decimal Quantite { get; private set; }

    public Money PrixUnitaire { get; private set; } = null!;

    public Money MontantLigne => PrixUnitaire.Multiply(Quantite);

    private LigneDevis()
    {
    }

    private LigneDevis(Guid id, string description, decimal quantite, Money prixUnitaire)
        : base(id)
    {
        Description = description;
        Quantite = quantite;
        PrixUnitaire = prixUnitaire;
    }

    public static Result<LigneDevis> Create(string description, decimal quantite, Money prixUnitaire)
    {
        Guard.Against.NullOrWhiteSpace(description);

        if (quantite <= 0)
        {
            return Result.Failure<LigneDevis>(Error.Validation("LigneDevis.QuantiteInvalide", "La quantité doit être strictement positive."));
        }

        if (prixUnitaire.Amount < 0)
        {
            return Result.Failure<LigneDevis>(Error.Validation("LigneDevis.PrixNegatif", "Le prix unitaire ne peut pas être négatif."));
        }

        return Result.Success(new LigneDevis(Guid.NewGuid(), description.Trim(), quantite, prixUnitaire));
    }
}
