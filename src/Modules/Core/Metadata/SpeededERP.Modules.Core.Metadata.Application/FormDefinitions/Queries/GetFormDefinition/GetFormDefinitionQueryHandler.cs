using MediatR;
using SpeededERP.Modules.Core.Metadata.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Metadata.Application.FormDefinitions.Queries.GetFormDefinition;

public sealed class GetFormDefinitionQueryHandler(IFormDefinitionRepository repository)
    : IRequestHandler<GetFormDefinitionQuery, Result<FormDefinitionDto>>
{
    public async Task<Result<FormDefinitionDto>> Handle(GetFormDefinitionQuery request, CancellationToken cancellationToken)
    {
        var form = await repository.GetByIdAsync(request.FormDefinitionId, cancellationToken).ConfigureAwait(false);
        if (form is null)
        {
            return Result.Failure<FormDefinitionDto>(Error.NotFound("FormDefinition.NotFound", "Formulaire introuvable."));
        }

        return Result.Success(new FormDefinitionDto(form.Id, form.EntityDefinitionId, form.Name, form.LayoutJson, form.Version, form.IsPublished));
    }
}
