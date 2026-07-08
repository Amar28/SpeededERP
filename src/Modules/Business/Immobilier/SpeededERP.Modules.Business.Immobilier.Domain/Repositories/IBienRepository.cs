using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Business.Immobilier.Domain.Aggregates.Biens;

namespace SpeededERP.Modules.Business.Immobilier.Domain.Repositories;

public interface IBienRepository : IRepository<Bien, Guid>;
