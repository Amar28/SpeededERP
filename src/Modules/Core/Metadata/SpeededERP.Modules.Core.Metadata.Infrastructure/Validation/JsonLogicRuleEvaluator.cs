using System.Text.Json;
using SpeededERP.Modules.Core.Metadata.Application.Validation;

namespace SpeededERP.Modules.Core.Metadata.Infrastructure.Validation;

/// <summary>
/// Minimal JsonLogic interpreter (architecture section 13): a fixed, constrained
/// grammar — <c>==, !=, &gt;, &gt;=, &lt;, &lt;=, and, or, !, if, in, var, missing</c> — never
/// arbitrary code execution. Unsupported operators fail closed (return false) rather
/// than throwing, so a malformed tenant-authored rule cannot crash the request.
/// </summary>
public sealed class JsonLogicRuleEvaluator : IRuleEvaluator
{
    public bool Evaluate(string ruleJson, JsonElement data)
    {
        try
        {
            using var document = JsonDocument.Parse(ruleJson);
            var result = EvaluateNode(document.RootElement, data);
            return result.ValueKind == JsonValueKind.True
                || (result.ValueKind == JsonValueKind.Number && result.GetDouble() != 0)
                || (result.ValueKind == JsonValueKind.String && result.GetString()?.Length > 0);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static JsonElement EvaluateNode(JsonElement node, JsonElement data)
    {
        if (node.ValueKind != JsonValueKind.Object)
        {
            return node;
        }

        var operation = node.EnumerateObject().FirstOrDefault();
        if (operation.Value.ValueKind == JsonValueKind.Undefined)
        {
            return node;
        }

        var args = operation.Value.ValueKind == JsonValueKind.Array
            ? operation.Value.EnumerateArray().ToArray()
            : [operation.Value];

        return operation.Name switch
        {
            "var" => ResolveVar(args.Length > 0 ? args[0] : default, data),
            "==" => Bool(JsonEquals(EvaluateNode(args[0], data), EvaluateNode(args[1], data))),
            "!=" => Bool(!JsonEquals(EvaluateNode(args[0], data), EvaluateNode(args[1], data))),
            "!" => Bool(!AsBool(EvaluateNode(args[0], data))),
            "and" => Bool(args.All(a => AsBool(EvaluateNode(a, data)))),
            "or" => Bool(args.Any(a => AsBool(EvaluateNode(a, data)))),
            ">" => Bool(AsNumber(EvaluateNode(args[0], data)) > AsNumber(EvaluateNode(args[1], data))),
            ">=" => Bool(AsNumber(EvaluateNode(args[0], data)) >= AsNumber(EvaluateNode(args[1], data))),
            "<" => Bool(AsNumber(EvaluateNode(args[0], data)) < AsNumber(EvaluateNode(args[1], data))),
            "<=" => Bool(AsNumber(EvaluateNode(args[0], data)) <= AsNumber(EvaluateNode(args[1], data))),
            "if" => EvaluateIf(args, data),
            "in" => Bool(EvaluateIn(args, data)),
            "missing" => EvaluateMissing(args, data),
            _ => Bool(false),
        };
    }

    private static JsonElement EvaluateIf(JsonElement[] args, JsonElement data)
    {
        for (var i = 0; i + 1 < args.Length; i += 2)
        {
            if (AsBool(EvaluateNode(args[i], data)))
            {
                return EvaluateNode(args[i + 1], data);
            }
        }

        return args.Length % 2 == 1 ? EvaluateNode(args[^1], data) : Bool(false);
    }

    private static bool EvaluateIn(JsonElement[] args, JsonElement data)
    {
        var needle = EvaluateNode(args[0], data);
        var haystack = EvaluateNode(args[1], data);

        if (haystack.ValueKind != JsonValueKind.Array)
        {
            return false;
        }

        return haystack.EnumerateArray().Any(item => JsonEquals(item, needle));
    }

    private static JsonElement EvaluateMissing(JsonElement[] args, JsonElement data)
    {
        var missing = args
            .Select(a => a.ValueKind == JsonValueKind.String ? a.GetString() : null)
            .Where(key => key is not null)
            .Where(key => data.ValueKind != JsonValueKind.Object || !data.TryGetProperty(key!, out _))
            .ToArray();

        return JsonSerializer.SerializeToElement(missing);
    }

    private static JsonElement ResolveVar(JsonElement pathNode, JsonElement data)
    {
        var path = pathNode.ValueKind == JsonValueKind.String ? pathNode.GetString() : null;
        if (string.IsNullOrEmpty(path))
        {
            return data;
        }

        var current = data;
        foreach (var segment in path.Split('.'))
        {
            if (current.ValueKind != JsonValueKind.Object || !current.TryGetProperty(segment, out current))
            {
                return default;
            }
        }

        return current;
    }

    private static bool AsBool(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Number => element.GetDouble() != 0,
        JsonValueKind.String => !string.IsNullOrEmpty(element.GetString()),
        JsonValueKind.Null or JsonValueKind.Undefined => false,
        _ => true,
    };

    private static double AsNumber(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.Number => element.GetDouble(),
        JsonValueKind.String when double.TryParse(element.GetString(), out var parsed) => parsed,
        _ => 0,
    };

    private static bool JsonEquals(JsonElement left, JsonElement right)
    {
        if (left.ValueKind == JsonValueKind.Number && right.ValueKind == JsonValueKind.Number)
        {
            return left.GetDouble().Equals(right.GetDouble());
        }

        if (left.ValueKind is JsonValueKind.True or JsonValueKind.False
            || right.ValueKind is JsonValueKind.True or JsonValueKind.False)
        {
            return AsBool(left) == AsBool(right);
        }

        return string.Equals(left.ToString(), right.ToString(), StringComparison.Ordinal);
    }

    private static JsonElement Bool(bool value) => value ? True : False;

    private static readonly JsonElement True = JsonSerializer.SerializeToElement(true);
    private static readonly JsonElement False = JsonSerializer.SerializeToElement(false);
}
