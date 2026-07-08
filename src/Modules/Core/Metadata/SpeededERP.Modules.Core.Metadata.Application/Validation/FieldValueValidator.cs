using System.Text.Json;
using SpeededERP.Modules.Core.Metadata.Domain.Aggregates.FieldDefinitions;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Metadata.Application.Validation;

/// <summary>
/// Server-side gate every write to <c>custom_fields JSONB</c> passes through — UI,
/// REST, GraphQL and bulk import alike (architecture section 10): "le no-code ne
/// dispense jamais de la validation". Checks required-ness, type coherence, List
/// options, and any declarative JsonLogic rule attached to the field.
/// </summary>
public sealed class FieldValueValidator(IRuleEvaluator ruleEvaluator)
{
    public Result Validate(IReadOnlyCollection<FieldDefinition> fieldDefinitions, JsonElement customFieldsPayload)
    {
        var errors = new List<string>();

        foreach (var field in fieldDefinitions)
        {
            var value = default(JsonElement);
            var hasValue = customFieldsPayload.ValueKind == JsonValueKind.Object
                && customFieldsPayload.TryGetProperty(field.Key, out value)
                && value.ValueKind is not JsonValueKind.Null and not JsonValueKind.Undefined;

            if (field.IsRequired && !hasValue)
            {
                errors.Add($"Le champ '{field.Key}' est obligatoire.");
                continue;
            }

            if (!hasValue)
            {
                continue;
            }

            if (!IsTypeCoherent(field.Type, value))
            {
                errors.Add($"Le champ '{field.Key}' n'a pas le type attendu ({field.Type}).");
                continue;
            }

            if (field.Type == FieldType.List && value.ValueKind == JsonValueKind.String
                && field.ListOptions.Count > 0 && !field.ListOptions.Contains(value.GetString()!))
            {
                errors.Add($"La valeur du champ '{field.Key}' ne fait pas partie des options autorisées.");
            }

            if (!string.IsNullOrWhiteSpace(field.ValidationRuleJson)
                && !ruleEvaluator.Evaluate(field.ValidationRuleJson, customFieldsPayload))
            {
                errors.Add($"Le champ '{field.Key}' ne respecte pas la règle de validation configurée.");
            }
        }

        return errors.Count == 0
            ? Result.Success()
            : Result.Failure(Error.Validation("FieldValue.Invalid", string.Join(" | ", errors)));
    }

    private static bool IsTypeCoherent(FieldType type, JsonElement value) => type switch
    {
        FieldType.Number => value.ValueKind == JsonValueKind.Number,
        FieldType.TextBox or FieldType.List or FieldType.QrCode => value.ValueKind == JsonValueKind.String,
        FieldType.Date => value.ValueKind == JsonValueKind.String && DateTimeOffset.TryParse(value.GetString(), out _),
        FieldType.Upload or FieldType.Signature or FieldType.Scanner => value.ValueKind == JsonValueKind.String,
        FieldType.Map or FieldType.Gps => value.ValueKind == JsonValueKind.Object,
        _ => true,
    };
}
