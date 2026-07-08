using System.Text.Json;
using SpeededERP.SharedKernel;

namespace SpeededERP.BuildingBlocks.Application.Validation;

/// <summary>
/// Shared port so any module can validate a <c>custom_fields</c> JSONB payload against
/// the tenant's FieldDefinitions without referencing Core.Metadata's assembly
/// (architecture sections 06 and 10 — no module references another module's
/// internals). Implemented in Core.Metadata.Infrastructure, wired here only by
/// interface; the concrete registration happens once in Host.Api's composition root.
/// </summary>
public interface ICustomFieldsValidator
{
    Task<Result> ValidateAsync(
        string moduleName,
        string entityName,
        Guid tenantId,
        JsonElement customFields,
        CancellationToken cancellationToken = default);
}
