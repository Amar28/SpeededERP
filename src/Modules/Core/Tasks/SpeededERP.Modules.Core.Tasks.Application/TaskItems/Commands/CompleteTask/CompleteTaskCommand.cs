using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Core.Tasks.Application.TaskItems.Commands.CompleteTask;

public sealed record CompleteTaskCommand(Guid TaskId) : ICommand;
