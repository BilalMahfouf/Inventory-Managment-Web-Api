using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common.Events;

public abstract record DomainEvent() : INotification, IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();

    
}
