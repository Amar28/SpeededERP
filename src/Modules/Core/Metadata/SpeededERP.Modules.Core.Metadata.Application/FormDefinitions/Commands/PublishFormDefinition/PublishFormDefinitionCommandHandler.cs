using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Metadata.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Metadata.Application.FormDefinitions.Commands.PublishFormDefinition;

public sealed class PublishFormDefinitionCommandHandler(IFormDefinitionRepository repository, [FromKeyedServices("metadata")] IUnitOfWork unitOfWork)
    : IRequestHandler<PublishFormDefinitionCommand, Result>
{
    public async Task<Result> Handle(PublishFormDefinitionCommand request, CancellationToken cancellationToken)
    {
        var form = await repository.GetByIdAsync(request.FormDefinitionId, cancellationToken).ConfigureAwait(false);
        if (form is null)
        {
            return Result.Failure(Error.NotFound("FormDefinition.NotFound", "Formulaire introuvable."));
        }

        form.Publish();
        repository.Update(form);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success();
    }
}
