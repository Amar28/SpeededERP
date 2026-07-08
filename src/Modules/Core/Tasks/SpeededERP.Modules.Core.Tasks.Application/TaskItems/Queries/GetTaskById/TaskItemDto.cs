using SpeededERP.Modules.Core.Tasks.Domain.Aggregates.TaskItems;

namespace SpeededERP.Modules.Core.Tasks.Application.TaskItems.Queries.GetTaskById;

public sealed record TaskItemDto(
    Guid Id,
    string Title,
    string? Description,
    Guid? AssigneeUserId,
    DateTimeOffset? DueDateUtc,
    SpeededErpTaskStatus Status);
