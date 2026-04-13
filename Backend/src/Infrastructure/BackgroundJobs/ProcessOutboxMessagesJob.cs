using Domain.Shared.Events;
using Infrastructure.Outbox;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Quartz;

namespace Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public class ProcessOutboxMessagesJob : IJob
{
    private readonly InventoryManagmentDBContext _dbContext;
    private readonly IDomainEventPublisher _publisher;

    public ProcessOutboxMessagesJob(
        IDomainEventPublisher publisher,
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
            .OrderBy(e => e.Id)
            .Where(e => e.ProcessedOnUtc == null)
            .Take(20)
            .ToListAsync(context.CancellationToken);
        if (outboxMessages is null || !outboxMessages.Any())
        {
            return;
        }

        foreach (var outboxMessage in outboxMessages)
        {
            var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                outboxMessage.Content, _serializerSettings);
            if (domainEvent is null)
            {
                continue;
            }
            await _publisher.PublishAsync(domainEvent, context.CancellationToken);
            outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
        }
        await _dbContext.SaveChangesAsync(context.CancellationToken);

    }
}
