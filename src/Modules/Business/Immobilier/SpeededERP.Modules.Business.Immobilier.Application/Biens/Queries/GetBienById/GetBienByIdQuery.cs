using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Business.Immobilier.Application.Biens.Queries.GetBienById;

public sealed record GetBienByIdQuery(Guid BienId) : IQuery<BienDto>;
