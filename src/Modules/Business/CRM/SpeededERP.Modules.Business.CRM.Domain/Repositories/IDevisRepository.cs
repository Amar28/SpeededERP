using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Business.CRM.Domain.Aggregates.Devis;

namespace SpeededERP.Modules.Business.CRM.Domain.Repositories;

public interface IDevisRepository : IRepository<Devis, Guid>;
