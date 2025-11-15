using System;
using System.Collections.Generic;
using System.Text;
using Domain.Inventories;
using MediatR;

namespace Application.Inventories.DomainEventsHandlers;

public sealed class LowStockDomainEventHandler
    : INotificationHandler<LowStockDomainEvent>

{
    // implemnt real time notification here with signalR
    public Task Handle(
        LowStockDomainEvent notification,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("the background job is working and the event is published ");
        return Task.CompletedTask;
    }
}
