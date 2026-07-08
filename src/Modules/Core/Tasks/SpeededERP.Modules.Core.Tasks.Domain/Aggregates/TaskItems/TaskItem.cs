using Ardalis.GuardClauses;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Tasks.Domain.Aggregates.TaskItems;

/// <summary>Named to avoid colliding with <see cref="System.Threading.Tasks.TaskStatus"/>.</summary>
public enum SpeededErpTaskStatus
{
    Open,
    InProgress,
    Done,
    Cancelled,
}

/// <summary>
/// A lightweight, assignable to-do item surfaced across every module's screens
/// (architecture section 20). Deliberately generic — unlike CRM's Devis it carries no
/// line items, just a small state machine over <see cref="SpeededErpTaskStatus"/>.
/// </summary>
public sealed class TaskItem : AggregateRoot<Guid>, IAuditable, IMustHaveTenant
{
    public string Title { get; private set; } = null!;

    public string? Description { get; private set; }

    public Guid? AssigneeUserId { get; private set; }

    public DateTimeOffset? DueDateUtc { get; private set; }

    public SpeededErpTaskStatus Status { get; private set; }

    public Guid TenantId { get; private set; }

    public DateTimeOffset CreatedOnUtc { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTimeOffset? LastModifiedOnUtc { get; private set; }

    public string? LastModifiedBy { get; private set; }

    private TaskItem()
    {
    }

    private TaskItem(Guid id, string title, string? description, Guid? assigneeUserId, DateTimeOffset? dueDateUtc)
        : base(id)
    {
        Title = title;
        Description = description;
        AssigneeUserId = assigneeUserId;
        DueDateUtc = dueDateUtc;
        Status = SpeededErpTaskStatus.Open;
    }

    public static Result<TaskItem> Create(string title, string? description, Guid? assigneeUserId, DateTimeOffset? dueDateUtc)
    {
        Guard.Against.NullOrWhiteSpace(title);

        return Result.Success(new TaskItem(Guid.NewGuid(), title.Trim(), description, assigneeUserId, dueDateUtc));
    }

    public Result Assign(Guid userId)
    {
        if (Status is SpeededErpTaskStatus.Done or SpeededErpTaskStatus.Cancelled)
        {
            return Result.Failure(Error.Conflict("TaskItem.EtatInvalide", "Impossible d'assigner une tâche terminée ou annulée."));
        }

        AssigneeUserId = userId;
        return Result.Success();
    }

    public Result Start()
    {
        if (Status != SpeededErpTaskStatus.Open)
        {
            return Result.Failure(Error.Conflict("TaskItem.EtatInvalide", "Seule une tâche ouverte peut être démarrée."));
        }

        Status = SpeededErpTaskStatus.InProgress;
        return Result.Success();
    }

    public Result Complete()
    {
        if (Status is not (SpeededErpTaskStatus.Open or SpeededErpTaskStatus.InProgress))
        {
            return Result.Failure(Error.Conflict("TaskItem.EtatInvalide", "Seule une tâche ouverte ou en cours peut être terminée."));
        }

        Status = SpeededErpTaskStatus.Done;
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status is SpeededErpTaskStatus.Done or SpeededErpTaskStatus.Cancelled)
        {
            return Result.Failure(Error.Conflict("TaskItem.EtatInvalide", "Impossible d'annuler une tâche déjà terminée ou annulée."));
        }

        Status = SpeededErpTaskStatus.Cancelled;
        return Result.Success();
    }

    public void AssignTenant(Guid tenantId) => TenantId = tenantId;

    public void SetCreated(DateTimeOffset onUtc, string? by)
    {
        CreatedOnUtc = onUtc;
        CreatedBy = by;
    }

    public void SetModified(DateTimeOffset onUtc, string? by)
    {
        LastModifiedOnUtc = onUtc;
        LastModifiedBy = by;
    }
}
