using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Metadata.Domain.Aggregates.FormDefinitions;
using SpeededERP.Modules.Core.Metadata.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Metadata.Application.FormDefinitions.Commands.SaveFormDefinition;

public sealed class SaveFormDefinitionCommandHandler(IFormDefinitionRepository repository, [FromKeyedServices("metadata")] IUnitOfWork unitOfWork)
    : IRequestHandler<SaveFormDefinitionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(SaveFormDefinitionCommand request, CancellationToken cancellationToken)
    {
        if (request.FormDefinitionId is { } id)
        {
            var existing = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (existing is null)
            {
                return Result.Failure<Guid>(Error.NotFound("FormDefinition.NotFound", "Formulaire introuvable."));
            }

            var updateResult = existing.UpdateLayout(request.LayoutJson);
            if (updateResult.IsFailure)
            {
                return Result.Failure<Guid>(updateResult.Error);
            }

            repository.Update(existing);
            await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success(existing.Id);
        }

        var createResult = FormDefinition.Create(request.EntityDefinitionId, request.Name, request.LayoutJson);
        if (createResult.IsFailure)
        {
            return Result.Failure<Guid>(createResult.Error);
        }

        repository.Add(createResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(createResult.Value.Id);
    }
}
