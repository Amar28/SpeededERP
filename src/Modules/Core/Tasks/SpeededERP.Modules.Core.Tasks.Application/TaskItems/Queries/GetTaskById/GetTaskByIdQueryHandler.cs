using MediatR;
using SpeededERP.Modules.Core.Tasks.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Tasks.Application.TaskItems.Queries.GetTaskById;

public sealed class GetTaskByIdQueryHandler(ITaskItemRepository repository)
    : IRequestHandler<GetTaskByIdQuery, Result<TaskItemDto>>
{
    public async Task<Result<TaskItemDto>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await repository.GetByIdAsync(request.TaskId, cancellationToken).ConfigureAwait(false);
        if (task is null)
        {
            return Result.Failure<TaskItemDto>(Error.NotFound("TaskItem.NotFound", "Tâche introuvable."));
        }

        return Result.Success(new TaskItemDto(task.Id, task.Title, task.Description, task.AssigneeUserId, task.DueDateUtc, task.Status));
    }
}
