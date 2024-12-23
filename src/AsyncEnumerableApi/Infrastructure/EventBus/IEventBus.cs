namespace AsyncEnumerableApi.Infrastructure.EventBus;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : class;

    IDisposable Subscribe<TEvent>(Action<TEvent> handler) 
        where TEvent : class;

    IDisposable Subscribe<TEvent>(Func<TEvent, Task> handler) 
        where TEvent : class;
}