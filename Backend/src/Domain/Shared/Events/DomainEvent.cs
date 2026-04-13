namespace Domain.Shared.Events;

public abstract record DomainEvent() : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}
