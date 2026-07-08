using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Business.CRM.Application.Devis.Queries.GetDevisById;

public sealed record GetDevisByIdQuery(Guid DevisId) : IQuery<DevisDto>;
