using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Tasks.Domain.Aggregates.TaskItems;

namespace SpeededERP.Modules.Core.Tasks.Domain.Repositories;

public interface ITaskItemRepository : IRepository<TaskItem, Guid>
{
}
