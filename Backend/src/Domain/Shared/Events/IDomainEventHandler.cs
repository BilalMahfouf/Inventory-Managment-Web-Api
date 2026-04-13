namespace Domain.Shared.Events;

public interface IDomainEventHandler<in T>
    where T : IDomainEvent
{
    Task Handle(T domainEvent, CancellationToken cancellationToken);
}
