using System.Collections.Concurrent;
using Domain.Shared.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.CQRS;

internal sealed class DomainEventsDispatcher(IServiceProvider serviceProvider)
    : IDomainEventDispatcher
{
    private static readonly ConcurrentDictionary<Type, Type> HandlerTypeDictionary = new();
    private static readonly ConcurrentDictionary<Type, Type> WrapperTypeDictionary = new();

    public async Task DispatchAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        foreach (IDomainEvent domainEvent in domainEvents)
        {
            using IServiceScope scope = serviceProvider.CreateScope();

            Type domainEventType = domainEvent.GetType();
            Type handlerType = HandlerTypeDictionary.GetOrAdd(
                domainEventType,
                eventType => typeof(IDomainEventHandler<>).MakeGenericType(eventType));

            IEnumerable<object?> handlers = scope.ServiceProvider.GetServices(handlerType);

            foreach (object? handler in handlers)
            {
                if (handler is null)
                {
                    continue;
                }

                var wrapper = HandlerWrapper.Create(handler, domainEventType);
                await wrapper.Handle(domainEvent, cancellationToken);
            }
        }
    }

    private abstract class HandlerWrapper
    {
        public abstract Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken);

        public static HandlerWrapper Create(object handler, Type domainEventType)
        {
            Type wrapperType = WrapperTypeDictionary.GetOrAdd(
                domainEventType,
                eventType => typeof(HandlerWrapper<>).MakeGenericType(eventType));

            return (HandlerWrapper)Activator.CreateInstance(wrapperType, handler)!;
        }
    }

    private sealed class HandlerWrapper<T>(object handler)
        : HandlerWrapper where T : IDomainEvent
    {
        private readonly IDomainEventHandler<T> _handler = (IDomainEventHandler<T>)handler;

        public override async Task Handle(
            IDomainEvent domainEvent,
            CancellationToken cancellationToken)
        {
            await _handler.Handle((T)domainEvent, cancellationToken);
        }
    }
}
