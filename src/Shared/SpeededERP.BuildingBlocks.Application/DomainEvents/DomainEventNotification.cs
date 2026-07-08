using MediatR;
using SpeededERP.SharedKernel;

namespace SpeededERP.BuildingBlocks.Application.DomainEvents;

/// <summary>
/// Adapts a pure <see cref="IDomainEvent"/> (no MediatR dependency, per section 31) into
/// a MediatR notification so Application-layer handlers can react to it in-process.
/// </summary>
public sealed class DomainEventNotification<TDomainEvent>(TDomainEvent domainEvent) : INotification
    where TDomainEvent : IDomainEvent
{
    public TDomainEvent DomainEvent { get; } = domainEvent;
}
