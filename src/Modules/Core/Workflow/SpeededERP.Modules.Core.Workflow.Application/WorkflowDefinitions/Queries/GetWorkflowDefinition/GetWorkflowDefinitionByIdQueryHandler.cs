using MediatR;
using SpeededERP.Modules.Core.Workflow.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Workflow.Application.WorkflowDefinitions.Queries.GetWorkflowDefinition;

public sealed class GetWorkflowDefinitionByIdQueryHandler(IWorkflowDefinitionRepository repository)
    : IRequestHandler<GetWorkflowDefinitionByIdQuery, Result<WorkflowDefinitionDto>>
{
    public async Task<Result<WorkflowDefinitionDto>> Handle(GetWorkflowDefinitionByIdQuery request, CancellationToken cancellationToken)
    {
        var workflowDefinition = await repository.GetByIdAsync(request.WorkflowDefinitionId, cancellationToken).ConfigureAwait(false);
        if (workflowDefinition is null)
        {
            return Result.Failure<WorkflowDefinitionDto>(Error.NotFound("WorkflowDefinition.NotFound", "Workflow introuvable."));
        }

        return Result.Success(new WorkflowDefinitionDto(
            workflowDefinition.Id,
            workflowDefinition.Name,
            workflowDefinition.EntityDefinitionId,
            workflowDefinition.DefinitionJson,
            workflowDefinition.IsActive));
    }
}
