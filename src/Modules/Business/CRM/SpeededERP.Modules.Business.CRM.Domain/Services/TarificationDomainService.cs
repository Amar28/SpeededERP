using SpeededERP.Modules.Business.CRM.Domain.Aggregates.Devis;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.CRM.Domain.Services;

/// <summary>
/// Domain logic that doesn't naturally belong to a single aggregate instance — here,
/// a volume discount that depends on policy rather than on any one Devis's state.
/// </summary>
public sealed class TarificationDomainService
{
    private const decimal SeuilRemiseVolume = 10_000m;
    private const decimal TauxRemiseVolume = 0.05m;

    public Money CalculerRemiseVolume(Devis devis)
    {
        var montant = devis.Montant;
        return montant.Amount >= SeuilRemiseVolume
            ? montant.Multiply(TauxRemiseVolume)
            : Money.Zero(montant.Currency);
    }
}
