using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Shared.Events;

public interface IDomainEvent : INotification
{
}
