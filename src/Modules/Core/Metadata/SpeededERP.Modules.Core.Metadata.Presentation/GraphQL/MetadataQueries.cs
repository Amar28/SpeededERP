using HotChocolate;
using HotChocolate.Types;
using MediatR;
using SpeededERP.Modules.Core.Metadata.Application.FormDefinitions.Queries.GetFormDefinition;

namespace SpeededERP.Modules.Core.Metadata.Presentation.GraphQL;

[ExtendObjectType(OperationTypeNames.Query)]
public sealed class MetadataQueries
{
    public async Task<FormDefinitionDto?> GetFormDefinition(Guid formDefinitionId, [Service] ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetFormDefinitionQuery(formDefinitionId), cancellationToken).ConfigureAwait(false);
        return result.IsSuccess ? result.Value : null;
    }
}
