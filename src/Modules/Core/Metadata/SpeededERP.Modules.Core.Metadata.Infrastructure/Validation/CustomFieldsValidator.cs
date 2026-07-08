using System.Text.Json;
using SpeededERP.BuildingBlocks.Application.Validation;
using SpeededERP.Modules.Core.Metadata.Application.Validation;
using SpeededERP.Modules.Core.Metadata.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Metadata.Infrastructure.Validation;

/// <summary>Concrete implementation of the shared <see cref="ICustomFieldsValidator"/> port — see that interface for why it lives here.</summary>
public sealed class CustomFieldsValidator(
    IEntityDefinitionRepository entityDefinitionRepository,
    IFieldDefinitionRepository fieldDefinitionRepository,
    FieldValueValidator fieldValueValidator)
    : ICustomFieldsValidator
{
    public async Task<Result> ValidateAsync(
        string moduleName,
        string entityName,
        Guid tenantId,
        JsonElement customFields,
        CancellationToken cancellationToken = default)
    {
        var entityDefinition = await entityDefinitionRepository
            .GetByModuleAndEntityAsync(moduleName, entityName, cancellationToken)
            .ConfigureAwait(false);

        if (entityDefinition is null)
        {
            // The owning module hasn't declared this entity yet — nothing to validate against.
            return Result.Success();
        }

        var fieldDefinitions = await fieldDefinitionRepository
            .GetByEntityDefinitionAsync(entityDefinition.Id, tenantId, cancellationToken)
            .ConfigureAwait(false);

        return fieldValueValidator.Validate(fieldDefinitions, customFields);
    }
}
