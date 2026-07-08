using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Business.Comptabilite.Domain.Aggregates.Factures;

namespace SpeededERP.Modules.Business.Comptabilite.Domain.Repositories;

public interface IFactureRepository : IRepository<Facture, Guid>;
