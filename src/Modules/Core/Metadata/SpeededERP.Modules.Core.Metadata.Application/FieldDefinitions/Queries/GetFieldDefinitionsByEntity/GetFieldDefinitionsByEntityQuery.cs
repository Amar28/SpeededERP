using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Core.Metadata.Application.FieldDefinitions.Queries.GetFieldDefinitionsByEntity;

/// <summary>
/// Read very frequently by every form render and by the FieldValueValidator on every
/// write — cached aggressively (architecture section 25), invalidated whenever a field
/// is added/edited in the Admin Portal. <paramref name="TenantId"/> is resolved by the
/// caller from <c>ITenantContext</c> rather than injected here, keeping this a plain
/// serializable MediatR message.
/// </summary>
public sealed record GetFieldDefinitionsByEntityQuery(Guid EntityDefinitionId, Guid TenantId)
    : IQuery<IReadOnlyCollection<FieldDefinitionDto>>, ICacheableQuery
{
    public string CacheKey => $"metadata:field-definitions:{TenantId}:{EntityDefinitionId}";

    public TimeSpan? Expiration => TimeSpan.FromMinutes(30);
}
