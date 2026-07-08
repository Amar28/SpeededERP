using SpeededERP.Modules.Business.Immobilier.Domain.ValueObjects;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.Immobilier.Domain.Aggregates.Biens;

/// <summary>The Immobilier module's reference aggregate (architecture section 07): a rentable property and its occupation state.</summary>
public sealed class Bien : AggregateRoot<Guid>, IAuditable, IMustHaveTenant
{
    public AdresseBien Adresse { get; private set; } = null!;

    public TypeBien TypeBien { get; private set; }

    public Money LoyerMensuel { get; private set; } = null!;

    public StatutOccupation StatutOccupation { get; private set; }

    public Guid TenantId { get; private set; }

    public DateTimeOffset CreatedOnUtc { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTimeOffset? LastModifiedOnUtc { get; private set; }

    public string? LastModifiedBy { get; private set; }

    private Bien()
    {
    }

    private Bien(Guid id, AdresseBien adresse, TypeBien typeBien, Money loyerMensuel)
        : base(id)
    {
        Adresse = adresse;
        TypeBien = typeBien;
        LoyerMensuel = loyerMensuel;
        StatutOccupation = StatutOccupation.Vacant;
    }

    public static Result<Bien> Enregistrer(AdresseBien adresse, TypeBien typeBien, Money loyerMensuel)
    {
        if (loyerMensuel.Amount < 0)
        {
            return Result.Failure<Bien>(Error.Validation("Bien.LoyerNegatif", "Le loyer mensuel ne peut pas être négatif."));
        }

        return Result.Success(new Bien(Guid.NewGuid(), adresse, typeBien, loyerMensuel));
    }

    public Result MettreEnLocation()
    {
        if (StatutOccupation != StatutOccupation.Vacant)
        {
            return Result.Failure(Error.Conflict("Bien.EtatInvalide", "Seul un bien vacant peut être mis en location."));
        }

        StatutOccupation = StatutOccupation.Loue;
        return Result.Success();
    }

    public Result LibererBien()
    {
        if (StatutOccupation != StatutOccupation.Loue)
        {
            return Result.Failure(Error.Conflict("Bien.EtatInvalide", "Seul un bien loué peut être libéré."));
        }

        StatutOccupation = StatutOccupation.Vacant;
        return Result.Success();
    }

    public Result MettreEnTravaux()
    {
        if (StatutOccupation == StatutOccupation.EnTravaux)
        {
            return Result.Failure(Error.Conflict("Bien.EtatInvalide", "Ce bien est déjà en travaux."));
        }

        StatutOccupation = StatutOccupation.EnTravaux;
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
