using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Business.RH.Application.Employes.Queries.GetEmployeById;

public sealed record GetEmployeByIdQuery(Guid EmployeId) : IQuery<EmployeDto>;
