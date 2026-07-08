using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Core.Tasks.Application.TaskItems.Queries.GetTaskById;

public sealed record GetTaskByIdQuery(Guid TaskId) : IQuery<TaskItemDto>;
