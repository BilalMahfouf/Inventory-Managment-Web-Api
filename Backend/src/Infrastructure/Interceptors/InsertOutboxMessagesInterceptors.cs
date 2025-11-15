using Domain.Entities.Common;
using Newtonsoft.Json;
using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

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
                           var domainEvents = e.DomainEvents;
                           e.ClearDomainEvents();
                           return domainEvents;
                       })
                       .Select(domainEvents => new OutboxMessages
                       {
                           Id = Guid.NewGuid(),
                           Name = domainEvents.GetType().Name,
                           Content = JsonConvert.SerializeObject(domainEvents,
                               new JsonSerializerSettings
                               {
                                   TypeNameHandling = TypeNameHandling.All
                               }),
                           CreatedOnUtc = DateTime.UtcNow
                       });
        context.Set<OutboxMessages>().AddRange(outboxMessages);
    }
}
