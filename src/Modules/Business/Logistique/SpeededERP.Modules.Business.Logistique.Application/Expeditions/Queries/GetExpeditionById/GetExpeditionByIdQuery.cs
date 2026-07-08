using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Business.Logistique.Application.Expeditions.Queries.GetExpeditionById;

public sealed record GetExpeditionByIdQuery(Guid ExpeditionId) : IQuery<ExpeditionDto>;
