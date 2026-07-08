using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Core.Tasks.Application.TaskItems.Commands.CreateTask;

public sealed record CreateTaskCommand(
    string Title,
    string? Description,
    Guid? AssigneeUserId,
    DateTimeOffset? DueDateUtc) : ICommand<Guid>;
