using Domain.Entities.Common;
using Newtonsoft.Json;
using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Domain.Common.Events;

namespace Infrastructure.Interceptors;

public class InsertOutboxMessagesInterceptors : SaveChangesInterceptor
{

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            InsertOutboxMessage(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    private void InsertOutboxMessage(DbContext context)
    {
        var outboxMessages = context.ChangeTracker
                       .Entries<AggregateRoot>()
                       .Select(entry => entry.Entity)
                       .SelectMany(e =>
                       {
                           var domainEvents = e.DomainEvents.ToList();
                           e.ClearDomainEvents();
                           return domainEvents;
                       })
                       .Select(@event => new OutboxMessages
                       {
                           Id = Guid.NewGuid(),
                           Name = @event.GetType().Name,
                           Content = JsonConvert.SerializeObject(@event,
                               new JsonSerializerSettings
                               {
                                   TypeNameHandling = TypeNameHandling.Auto
                               }),
                           CreatedOnUtc = DateTime.UtcNow
                       }).ToList();
        context.Set<OutboxMessages>().AddRange(outboxMessages);
    }
}
