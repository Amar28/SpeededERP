using System.Text.Json;

namespace SpeededERP.Modules.Core.Metadata.Application.Validation;

/// <summary>
/// Outbound port to the declarative rule engine (architecture section 13): a
/// constrained JsonLogic-style grammar, deliberately <b>not</b> an arbitrary code
/// interpreter — running untrusted tenant-authored code in-process would be a remote
/// code execution vector. Implemented in Infrastructure.
/// </summary>
public interface IRuleEvaluator
{
    /// <summary><paramref name="ruleJson"/> is a JsonLogic rule; <paramref name="data"/> is the record being validated.</summary>
    bool Evaluate(string ruleJson, JsonElement data);
}
