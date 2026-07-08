namespace SpeededERP.Modules.Core.Tasks.Presentation.Contracts;

public sealed record CreateTaskRequest(string Title, string? Description, Guid? AssigneeUserId, DateTimeOffset? DueDateUtc);
