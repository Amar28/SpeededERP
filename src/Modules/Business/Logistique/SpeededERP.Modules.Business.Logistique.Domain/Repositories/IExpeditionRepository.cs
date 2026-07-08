using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Business.Logistique.Domain.Aggregates.Expeditions;

namespace SpeededERP.Modules.Business.Logistique.Domain.Repositories;

public interface IExpeditionRepository : IRepository<Expedition, Guid>;
