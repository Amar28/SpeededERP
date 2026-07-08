using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using SpeededERP.Modules.Core.Metadata.Application.Validation;
using SpeededERP.Modules.Core.Metadata.Domain.Aggregates.FieldDefinitions;
using Xunit;

namespace SpeededERP.Modules.Core.Metadata.UnitTests.Validation;

/// <summary>
/// Reference test suite for the server-side gate every custom-field write passes
/// through (architecture section 10: "le no-code ne dispense jamais de la
/// validation"). IRuleEvaluator is mocked — this suite is about FieldValueValidator's
/// own logic (required-ness, type coherence, list options), not the rule engine.
/// </summary>
public sealed class FieldValueValidatorTests
{
    private readonly IRuleEvaluator _ruleEvaluator = Substitute.For<IRuleEvaluator>();

    private FieldValueValidator CreateSut() => new(_ruleEvaluator);

    private static FieldDefinition CreateField(string key, FieldType type, bool required = false, int order = 0)
    {
        var field = FieldDefinition.Create(Guid.NewGuid(), key, $"labels.{key}", type, order).Value;
        field.MarkRequired(required);
        return field;
    }

    [Fact]
    public void Validate_champ_requis_absent_echoue()
    {
        var fields = new[] { CreateField("numero_fiscal", FieldType.TextBox, required: true) };
        var payload = JsonDocument.Parse("{}").RootElement;

        var result = CreateSut().Validate(fields, payload);

        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("numero_fiscal");
    }

    [Fact]
    public void Validate_champ_requis_present_reussit()
    {
        var fields = new[] { CreateField("numero_fiscal", FieldType.TextBox, required: true) };
        var payload = JsonDocument.Parse("""{"numero_fiscal":"FR123456"}""").RootElement;

        var result = CreateSut().Validate(fields, payload);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Validate_champ_non_requis_absent_reussit()
    {
        var fields = new[] { CreateField("secteur", FieldType.List, required: false) };
        var payload = JsonDocument.Parse("{}").RootElement;

        var result = CreateSut().Validate(fields, payload);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Validate_type_incoherent_echoue()
    {
        var fields = new[] { CreateField("montant", FieldType.Number) };
        var payload = JsonDocument.Parse("""{"montant":"pas-un-nombre"}""").RootElement;

        var result = CreateSut().Validate(fields, payload);

        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("montant");
    }

    [Fact]
    public void Validate_valeur_hors_options_liste_echoue()
    {
        var field = CreateField("secteur", FieldType.List);
        field.SetListOptions(["Entreprise", "Particulier"]);
        var payload = JsonDocument.Parse("""{"secteur":"Association"}""").RootElement;

        var result = CreateSut().Validate([field], payload);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Validate_valeur_dans_options_liste_reussit()
    {
        var field = CreateField("secteur", FieldType.List);
        field.SetListOptions(["Entreprise", "Particulier"]);
        var payload = JsonDocument.Parse("""{"secteur":"Entreprise"}""").RootElement;

        var result = CreateSut().Validate([field], payload);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Validate_delegue_les_regles_declaratives_a_IRuleEvaluator()
    {
        var field = CreateField("numero_fiscal", FieldType.TextBox);
        field.SetValidationRule("""{"==":[{"var":"secteur"},"Entreprise"]}""");
        var payload = JsonDocument.Parse("""{"numero_fiscal":"FR1","secteur":"Particulier"}""").RootElement;

        _ruleEvaluator.Evaluate(Arg.Any<string>(), Arg.Any<JsonElement>()).Returns(false);

        var result = CreateSut().Validate([field], payload);

        result.IsFailure.Should().BeTrue();
        _ruleEvaluator.Received(1).Evaluate(field.ValidationRuleJson!, Arg.Any<JsonElement>());
    }
}
