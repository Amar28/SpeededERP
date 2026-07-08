using FluentAssertions;
using SpeededERP.Modules.Business.CRM.Domain.Aggregates.Devis;
using SpeededERP.Modules.Business.CRM.Domain.Events;
using SpeededERP.SharedKernel;
using Xunit;

namespace SpeededERP.Modules.Business.CRM.UnitTests.Devis;

/// <summary>
/// Reference unit test suite for the module walked through in architecture sections
/// 07 and 09 — exercises the aggregate's invariants and state machine in isolation,
/// with no database, no MediatR, no HTTP: pure Domain-layer testing.
/// </summary>
public sealed class DevisTests
{
    private static SpeededERP.Modules.Business.CRM.Domain.Aggregates.Devis.Devis CreerDevisValide()
    {
        var devis = SpeededERP.Modules.Business.CRM.Domain.Aggregates.Devis.Devis.Creer(Guid.NewGuid(), "EUR").Value;
        devis.AjouterLigne("Prestation", 2, Money.Of(100m, "EUR"));
        return devis;
    }

    [Fact]
    public void Creer_avec_client_valide_produit_un_devis_en_brouillon()
    {
        var result = SpeededERP.Modules.Business.CRM.Domain.Aggregates.Devis.Devis.Creer(Guid.NewGuid(), "EUR");

        result.IsSuccess.Should().BeTrue();
        result.Value.Statut.Should().Be(StatutDevis.Brouillon);
        result.Value.Montant.Amount.Should().Be(0);
    }

    [Fact]
    public void AjouterLigne_calcule_correctement_le_montant_total()
    {
        var devis = SpeededERP.Modules.Business.CRM.Domain.Aggregates.Devis.Devis.Creer(Guid.NewGuid(), "EUR").Value;

        devis.AjouterLigne("Ligne A", 2, Money.Of(50m, "EUR"));
        devis.AjouterLigne("Ligne B", 1, Money.Of(30m, "EUR"));

        devis.Montant.Amount.Should().Be(130m);
        devis.Montant.Currency.Should().Be("EUR");
    }

    [Fact]
    public void AjouterLigne_avec_devise_differente_du_devis_echoue()
    {
        var devis = SpeededERP.Modules.Business.CRM.Domain.Aggregates.Devis.Devis.Creer(Guid.NewGuid(), "EUR").Value;

        var result = devis.AjouterLigne("Ligne", 1, Money.Of(10m, "USD"));

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Devis.DeviseIncoherente");
    }

    [Fact]
    public void AjouterLigne_apres_envoi_est_refusee()
    {
        var devis = CreerDevisValide();
        devis.Envoyer();

        var result = devis.AjouterLigne("Ligne tardive", 1, Money.Of(10m, "EUR"));

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Devis.NonModifiable");
    }

    [Fact]
    public void Envoyer_sans_lignes_echoue()
    {
        var devis = SpeededERP.Modules.Business.CRM.Domain.Aggregates.Devis.Devis.Creer(Guid.NewGuid(), "EUR").Value;

        var result = devis.Envoyer();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Devis.Vide");
    }

    [Fact]
    public void Envoyer_un_devis_valide_change_son_statut_et_leve_un_evenement()
    {
        var devis = CreerDevisValide();

        var result = devis.Envoyer();

        result.IsSuccess.Should().BeTrue();
        devis.Statut.Should().Be(StatutDevis.Envoye);
        devis.DomainEvents.Should().ContainSingle(e => e is DevisEnvoyeDomainEvent);
    }

    [Fact]
    public void Accepter_un_devis_non_envoye_echoue()
    {
        var devis = CreerDevisValide();

        var result = devis.Accepter();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Devis.EtatInvalide");
    }

    [Fact]
    public void Accepter_un_devis_envoye_change_son_statut_et_leve_DevisAcceptedDomainEvent()
    {
        var devis = CreerDevisValide();
        devis.Envoyer();
        devis.ClearDomainEvents();

        var result = devis.Accepter();

        result.IsSuccess.Should().BeTrue();
        devis.Statut.Should().Be(StatutDevis.Accepte);

        var domainEvent = devis.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<DevisAcceptedDomainEvent>().Subject;
        domainEvent.DevisId.Should().Be(devis.Id);
        domainEvent.Montant.Should().Be(200m);
        domainEvent.Devise.Should().Be("EUR");
    }

    [Fact]
    public void Refuser_un_devis_envoye_change_son_statut()
    {
        var devis = CreerDevisValide();
        devis.Envoyer();

        var result = devis.Refuser();

        result.IsSuccess.Should().BeTrue();
        devis.Statut.Should().Be(StatutDevis.Refuse);
    }

    [Fact]
    public void Accepter_un_devis_deja_accepte_echoue()
    {
        var devis = CreerDevisValide();
        devis.Envoyer();
        devis.Accepter();

        var result = devis.Accepter();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Devis.EtatInvalide");
    }
}
