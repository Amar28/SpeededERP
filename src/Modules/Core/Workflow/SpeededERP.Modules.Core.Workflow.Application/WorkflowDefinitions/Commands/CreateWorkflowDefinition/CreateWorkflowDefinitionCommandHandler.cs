using MediatR;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Workflow.Domain.Aggregates.WorkflowDefinitions;
using SpeededERP.Modules.Core.Workflow.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Workflow.Application.WorkflowDefinitions.Commands.CreateWorkflowDefinition;

public sealed class CreateWorkflowDefinitionCommandHandler(IWorkflowDefinitionRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateWorkflowDefinitionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateWorkflowDefinitionCommand request, CancellationToken cancellationToken)
    {
        var createResult = WorkflowDefinition.Create(request.Name, request.EntityDefinitionId, request.DefinitionJson);
        if (createResult.IsFailure)
        {
            return Result.Failure<Guid>(createResult.Error);
        }

        repository.Add(createResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(createResult.Value.Id);
    }
}
