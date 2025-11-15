using Domain.Common.Events;
using Infrastructure.Outbox;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public class ProcessOutboxMessagesJob : IJob
{
    private readonly InventoryManagmentDBContext _dbContext;
    private readonly IPublisher _publisher;

    public ProcessOutboxMessagesJob(
        IPublisher publisher,
        InventoryManagmentDBContext context)
    {
        _publisher = publisher;
        _dbContext = context;
    }
    private static readonly JsonSerializerSettings _serializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    public async Task Execute(IJobExecutionContext context)
    {
        var outboxMessages = await _dbContext.Set<OutboxMessages>()
            .Where(e => e.ProcessedOnUtc == null)
            .Take(20)
            .ToListAsync(context.CancellationToken);
        if(outboxMessages is null || !outboxMessages.Any())
        {
            return;
        }

        foreach (var outboxMessage in outboxMessages)
        {
            var domainEventType = Type.GetType(outboxMessage.Name);
            var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                outboxMessage.Content, _serializerSettings);
            if (domainEvent is null)
            {
                continue;
            }
            await _publisher.Publish(domainEvent, context.CancellationToken);
            outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
        }
        await _dbContext.SaveChangesAsync(context.CancellationToken);

    }
}
