using MediatR;
using SpeededERP.BuildingBlocks.Application.Security;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Metadata.Domain.Aggregates.FieldDefinitions;
using SpeededERP.Modules.Core.Metadata.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Metadata.Application.FieldDefinitions.Commands.AddFieldDefinition;

public sealed class AddFieldDefinitionCommandHandler(
    IEntityDefinitionRepository entityDefinitionRepository,
    IFieldDefinitionRepository fieldDefinitionRepository,
    IUnitOfWork unitOfWork,
    ITenantContext tenantContext)
    : IRequestHandler<AddFieldDefinitionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddFieldDefinitionCommand request, CancellationToken cancellationToken)
    {
        var entityDefinition = await entityDefinitionRepository.GetByIdAsync(request.EntityDefinitionId, cancellationToken).ConfigureAwait(false);
        if (entityDefinition is null)
        {
            return Result.Failure<Guid>(Error.NotFound("EntityDefinition.NotFound", "Entité introuvable."));
        }

        if (!entityDefinition.SupportsCustomFields)
        {
            return Result.Failure<Guid>(Error.Conflict("EntityDefinition.CustomFieldsNotSupported", "Cette entité n'accepte pas de champs personnalisés."));
        }

        if (await fieldDefinitionRepository.KeyExistsAsync(request.EntityDefinitionId, tenantContext.TenantId, request.Key, cancellationToken).ConfigureAwait(false))
        {
            return Result.Failure<Guid>(Error.Conflict("FieldDefinition.KeyAlreadyUsed", "Cette clé de champ est déjà utilisée pour cette entité."));
        }

        var fieldResult = FieldDefinition.Create(request.EntityDefinitionId, request.Key, request.LabelKey, request.Type, request.Order);
        if (fieldResult.IsFailure)
        {
            return Result.Failure<Guid>(fieldResult.Error);
        }

        var field = fieldResult.Value;
        field.MarkRequired(request.IsRequired);

        if (request.ListOptions is { Count: > 0 })
        {
            var optionsResult = field.SetListOptions(request.ListOptions);
            if (optionsResult.IsFailure)
            {
                return Result.Failure<Guid>(optionsResult.Error);
            }
        }

        field.SetValidationRule(request.ValidationRuleJson);

        fieldDefinitionRepository.Add(field);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(field.Id);
    }
}
