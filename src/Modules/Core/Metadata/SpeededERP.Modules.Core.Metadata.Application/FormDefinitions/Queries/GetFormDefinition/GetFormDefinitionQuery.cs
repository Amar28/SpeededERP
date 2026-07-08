using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Core.Metadata.Application.FormDefinitions.Queries.GetFormDefinition;

public sealed record GetFormDefinitionQuery(Guid FormDefinitionId) : IQuery<FormDefinitionDto>, ICacheableQuery
{
    public string CacheKey => $"metadata:form-definition:{FormDefinitionId}";

    public TimeSpan? Expiration => TimeSpan.FromMinutes(30);
}
