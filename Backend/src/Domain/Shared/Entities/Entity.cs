using Domain.Shared.Abstractions;
using Domain.Shared.Events;
using Domain.Shared.Exceptions;

namespace Domain.Shared.Entities;

public abstract class Entity : ISoftDeletable
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? DeletedByUserId { get; set; }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public virtual void Delete(int? deletedByUserId = null)
    {
        if (IsDeleted)
        {
            throw new DomainException($"{this.GetType().Name} is already deleted");
        }

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedByUserId = deletedByUserId;
    }
}