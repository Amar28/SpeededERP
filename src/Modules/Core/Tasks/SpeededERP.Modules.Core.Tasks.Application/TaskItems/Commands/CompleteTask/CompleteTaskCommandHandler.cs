using MediatR;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Tasks.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Tasks.Application.TaskItems.Commands.CompleteTask;

public sealed class CompleteTaskCommandHandler(ITaskItemRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<CompleteTaskCommand, Result>
{
    public async Task<Result> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await repository.GetByIdAsync(request.TaskId, cancellationToken).ConfigureAwait(false);
        if (task is null)
        {
            return Result.Failure(Error.NotFound("TaskItem.NotFound", "Tâche introuvable."));
        }

        var completeResult = task.Complete();
        if (completeResult.IsFailure)
        {
            return completeResult;
        }

        repository.Update(task);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success();
    }
}
