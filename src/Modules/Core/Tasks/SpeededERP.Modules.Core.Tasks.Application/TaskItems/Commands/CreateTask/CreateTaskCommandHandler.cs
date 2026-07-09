using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Tasks.Domain.Repositories;
using SpeededERP.SharedKernel;
using TaskItemAggregate = SpeededERP.Modules.Core.Tasks.Domain.Aggregates.TaskItems.TaskItem;

namespace SpeededERP.Modules.Core.Tasks.Application.TaskItems.Commands.CreateTask;

public sealed class CreateTaskCommandHandler(ITaskItemRepository repository, [FromKeyedServices("tasks")] IUnitOfWork unitOfWork)
    : IRequestHandler<CreateTaskCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var createResult = TaskItemAggregate.Create(request.Title, request.Description, request.AssigneeUserId, request.DueDateUtc);
        if (createResult.IsFailure)
        {
            return Result.Failure<Guid>(createResult.Error);
        }

        repository.Add(createResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(createResult.Value.Id);
    }
}
