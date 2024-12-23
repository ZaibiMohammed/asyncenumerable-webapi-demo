using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace AsyncEnumerableApi.Infrastructure.EventBus;

public class InMemoryEventBus : IEventBus
{
    private readonly ILogger<InMemoryEventBus> _logger;
    private readonly ConcurrentDictionary<Type, List<Func<object, Task>>> _handlers;

    public InMemoryEventBus(ILogger<InMemoryEventBus> logger)
    {
        _logger = logger;
        _handlers = new ConcurrentDictionary<Type, List<Func<object, Task>>>();
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : class
    {
        var eventType = typeof(TEvent);
        if (_handlers.TryGetValue(eventType, out var handlers))
        {
            _logger.LogInformation(
                "Publishing event {EventType} to {HandlerCount} handlers",
                eventType.Name, handlers.Count);

            foreach (var handler in handlers)
            {
                try
                {
                    await handler(@event);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, 
                        "Error handling event {EventType} in handler",
                        eventType.Name);
                }
            }
        }
    }

    public IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : class
    {
        return Subscribe<TEvent>((@event) => 
        {
            handler(@event);
            return Task.CompletedTask;
        });
    }

    public IDisposable Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class
    {
        var eventType = typeof(TEvent);
        var handlers = _handlers.GetOrAdd(eventType, _ => new List<Func<object, Task>>());

        var wrappedHandler = new Func<object, Task>(obj => handler((TEvent)obj));
        handlers.Add(wrappedHandler);

        return new SubscriptionToken(() =>
        {
            if (_handlers.TryGetValue(eventType, out var list))
            {
                list.Remove(wrappedHandler);
            }
        });
    }

    private class SubscriptionToken : IDisposable
    {
        private readonly Action _unsubscribeAction;

        public SubscriptionToken(Action unsubscribeAction)
        {
            _unsubscribeAction = unsubscribeAction;
        }

        public void Dispose()
        {
            _unsubscribeAction();
        }
    }
}