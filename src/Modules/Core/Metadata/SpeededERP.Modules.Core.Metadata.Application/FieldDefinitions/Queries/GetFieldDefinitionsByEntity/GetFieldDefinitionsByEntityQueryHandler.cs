using MediatR;
using SpeededERP.Modules.Core.Metadata.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Metadata.Application.FieldDefinitions.Queries.GetFieldDefinitionsByEntity;

public sealed class GetFieldDefinitionsByEntityQueryHandler(IFieldDefinitionRepository repository)
    : IRequestHandler<GetFieldDefinitionsByEntityQuery, Result<IReadOnlyCollection<FieldDefinitionDto>>>
{
    public async Task<Result<IReadOnlyCollection<FieldDefinitionDto>>> Handle(
        GetFieldDefinitionsByEntityQuery request,
        CancellationToken cancellationToken)
    {
        var fields = await repository
            .GetByEntityDefinitionAsync(request.EntityDefinitionId, request.TenantId, cancellationToken)
            .ConfigureAwait(false);

        IReadOnlyCollection<FieldDefinitionDto> dtos = fields
            .OrderBy(f => f.Order)
            .Select(f => new FieldDefinitionDto(f.Id, f.Key, f.LabelKey, f.Type, f.IsRequired, f.Order, f.ListOptions))
            .ToList();

        return Result.Success(dtos);
    }
}
