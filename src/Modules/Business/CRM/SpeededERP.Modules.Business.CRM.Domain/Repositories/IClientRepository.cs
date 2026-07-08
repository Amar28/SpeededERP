using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Business.CRM.Domain.Aggregates.Clients;

namespace SpeededERP.Modules.Business.CRM.Domain.Repositories;

public interface IClientRepository : IRepository<Client, Guid>;
