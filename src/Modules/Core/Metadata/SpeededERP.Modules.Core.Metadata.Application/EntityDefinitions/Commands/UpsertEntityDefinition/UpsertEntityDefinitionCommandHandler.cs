using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Metadata.Domain.Aggregates.EntityDefinitions;
using SpeededERP.Modules.Core.Metadata.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Metadata.Application.EntityDefinitions.Commands.UpsertEntityDefinition;

public sealed class UpsertEntityDefinitionCommandHandler(IEntityDefinitionRepository repository, [FromKeyedServices("metadata")] IUnitOfWork unitOfWork)
    : IRequestHandler<UpsertEntityDefinitionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UpsertEntityDefinitionCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository
            .GetByModuleAndEntityAsync(request.ModuleName, request.EntityName, cancellationToken)
            .ConfigureAwait(false);

        if (existing is not null)
        {
            return Result.Success(existing.Id);
        }

        var definitionResult = EntityDefinition.Declare(
            request.ModuleName,
            request.EntityName,
            request.DisplayNameKey,
            request.SupportsCustomFields);

        if (definitionResult.IsFailure)
        {
            return Result.Failure<Guid>(definitionResult.Error);
        }

        repository.Add(definitionResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(definitionResult.Value.Id);
    }
}
