using System.Text.Json;
using FluentAssertions;
using SpeededERP.Modules.Core.Metadata.Infrastructure.Validation;
using Xunit;

namespace SpeededERP.Modules.Core.Metadata.UnitTests.Validation;

/// <summary>
/// Reference suite for the declarative rule engine (architecture section 13) — a
/// constrained JsonLogic grammar, never arbitrary code execution. Exercises the exact
/// example from the architecture doc: "Numéro fiscal obligatoire si Secteur = Entreprise".
/// </summary>
public sealed class JsonLogicRuleEvaluatorTests
{
    private readonly JsonLogicRuleEvaluator _sut = new();

    private static JsonElement Data(string json) => JsonDocument.Parse(json).RootElement;

    [Fact]
    public void Evaluate_egalite_vraie()
    {
        var result = _sut.Evaluate("""{"==":[{"var":"secteur"},"Entreprise"]}""", Data("""{"secteur":"Entreprise"}"""));

        result.Should().BeTrue();
    }

    [Fact]
    public void Evaluate_egalite_fausse()
    {
        var result = _sut.Evaluate("""{"==":[{"var":"secteur"},"Entreprise"]}""", Data("""{"secteur":"Particulier"}"""));

        result.Should().BeFalse();
    }

    [Fact]
    public void Evaluate_and_combine_plusieurs_conditions()
    {
        const string rule = """{"and":[{"==":[{"var":"secteur"},"Entreprise"]},{">":[{"var":"montant"},1000]}]}""";

        _sut.Evaluate(rule, Data("""{"secteur":"Entreprise","montant":1500}""")).Should().BeTrue();
        _sut.Evaluate(rule, Data("""{"secteur":"Entreprise","montant":500}""")).Should().BeFalse();
    }

    [Fact]
    public void Evaluate_or_combine_plusieurs_conditions()
    {
        const string rule = """{"or":[{"==":[{"var":"statut"},"urgent"]},{"==":[{"var":"statut"},"critique"]}]}""";

        _sut.Evaluate(rule, Data("""{"statut":"critique"}""")).Should().BeTrue();
        _sut.Evaluate(rule, Data("""{"statut":"normal"}""")).Should().BeFalse();
    }

    [Fact]
    public void Evaluate_comparaisons_numeriques()
    {
        _sut.Evaluate("""{">=":[{"var":"age"},18]}""", Data("""{"age":18}""")).Should().BeTrue();
        _sut.Evaluate("""{"<":[{"var":"age"},18]}""", Data("""{"age":18}""")).Should().BeFalse();
    }

    [Fact]
    public void Evaluate_in_verifie_appartenance_a_une_liste()
    {
        const string rule = """{"in":[{"var":"pays"},["FR","BE","CH"]]}""";

        _sut.Evaluate(rule, Data("""{"pays":"FR"}""")).Should().BeTrue();
        _sut.Evaluate(rule, Data("""{"pays":"DE"}""")).Should().BeFalse();
    }

    [Fact]
    public void Evaluate_regle_malformee_echoue_sans_lever_d_exception()
    {
        var act = () => _sut.Evaluate("{ceci n'est pas du json valide", Data("{}"));

        act.Should().NotThrow();
        act().Should().BeFalse();
    }

    [Fact]
    public void Evaluate_var_sur_chemin_absent_renvoie_faux_plutot_que_de_lever()
    {
        var result = _sut.Evaluate("""{"==":[{"var":"champ.inexistant"},"x"]}""", Data("{}"));

        result.Should().BeFalse();
    }
}
