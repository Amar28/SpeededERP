using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Business.Comptabilite.Application.Factures.Queries.GetFactureById;

public sealed record GetFactureByIdQuery(Guid FactureId) : IQuery<FactureDto>;
