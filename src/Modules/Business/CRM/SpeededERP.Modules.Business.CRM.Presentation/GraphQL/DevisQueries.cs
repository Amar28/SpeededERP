using HotChocolate;
using HotChocolate.Types;
using MediatR;
using SpeededERP.Modules.Business.CRM.Application.Devis.Queries.GetDevisById;

namespace SpeededERP.Modules.Business.CRM.Presentation.GraphQL;

/// <summary>Same Application handler as <c>DevisEndpoints.GetDevisById</c> — no business logic duplicated between REST and GraphQL (section 19).</summary>
[ExtendObjectType(OperationTypeNames.Query)]
public sealed class DevisQueries
{
    public async Task<DevisDto?> GetDevisById(Guid devisId, [Service] ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetDevisByIdQuery(devisId), cancellationToken).ConfigureAwait(false);
        return result.IsSuccess ? result.Value : null;
    }
}
