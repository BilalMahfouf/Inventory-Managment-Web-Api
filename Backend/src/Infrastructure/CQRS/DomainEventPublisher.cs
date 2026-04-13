using System.Collections.Concurrent;
using Domain.Shared.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.CQRS;

public class DomainEventPublisher : IDomainEventPublisher
{
    private readonly IServiceProvider _serviceProvider;

    private static readonly ConcurrentDictionary<Type, Type> HandlerTypeDictionary = new();

    public DomainEventPublisher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task PublishAsync(IDomainEvent domainEvent, CancellationToken ct = default)
    {
        var eventType = domainEvent.GetType();
        var handlerType = HandlerTypeDictionary.GetOrAdd(
            eventType,
            type => typeof(IDomainEventHandler<>).MakeGenericType(type));

        var handlers = _serviceProvider.GetServices(handlerType);

        if (!handlers.Any())
        {
            return;
        }

        var tasks = handlers.Select(handler =>
            InvokeHandlerAsync(handler!, domainEvent, ct));

        await Task.WhenAll(tasks);
    }

    private static async Task InvokeHandlerAsync(
        object handler,
        IDomainEvent domainEvent,
        CancellationToken ct)
    {
        await ((dynamic)handler).Handle((dynamic)domainEvent, ct);
    }
}
