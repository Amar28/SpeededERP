using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Business.RH.Domain.Aggregates.Employes;

namespace SpeededERP.Modules.Business.RH.Domain.Repositories;

public interface IEmployeRepository : IRepository<Employe, Guid>;
