using AsyncEnumerableApi.Infrastructure.EventBus;
using AsyncEnumerableApi.Infrastructure.Streaming.Events;
using Microsoft.Extensions.Caching.Memory;

namespace AsyncEnumerableApi.Infrastructure.Streaming.Handlers;

public class StreamingEventHandler : IDisposable
{
    private readonly ILogger<StreamingEventHandler> _logger;
    private readonly IMemoryCache _cache;
    private readonly List<IDisposable> _subscriptions;

    public StreamingEventHandler(
        IEventBus eventBus,
        ILogger<StreamingEventHandler> logger,
        IMemoryCache cache)
    {
        _logger = logger;
        _cache = cache;
        _subscriptions = new List<IDisposable>
        {
            eventBus.Subscribe<StreamStartedEvent>(HandleStreamStarted),
            eventBus.Subscribe<StreamProgressEvent>(HandleStreamProgress),
            eventBus.Subscribe<StreamCompletedEvent>(HandleStreamCompleted),
            eventBus.Subscribe<StreamErrorEvent>(HandleStreamError)
        };
    }

    private Task HandleStreamStarted(StreamStartedEvent @event)
    {
        _logger.LogInformation(
            "Stream {StreamId} started for type {ItemType} with batch size {BatchSize}",
            @event.StreamId, @event.ItemType.Name, @event.BatchSize);

        _cache.Set($"stream:{@event.StreamId}", new StreamingMetrics
        {
            StreamId = @event.StreamId,
            StartTime = @event.StartTime,
            ItemType = @event.ItemType.Name,
            BatchSize = @event.BatchSize
        });

        return Task.CompletedTask;
    }

    private Task HandleStreamProgress(StreamProgressEvent @event)
    {
        var metrics = _cache.Get<StreamingMetrics>($"stream:{@event.StreamId}");
        if (metrics != null)
        {
            metrics.ItemsProcessed = @event.ItemsProcessed;
            metrics.TotalItems = @event.TotalItems;
            metrics.ElapsedTime = @event.ElapsedTime;
            metrics.ItemsPerSecond = @event.ItemsProcessed / @event.ElapsedTime.TotalSeconds;

            _cache.Set($"stream:{@event.StreamId}", metrics);
        }

        _logger.LogInformation(
            "Stream {StreamId} progress: {ItemsProcessed}/{TotalItems} items in {ElapsedTime:g}",
            @event.StreamId, @event.ItemsProcessed, @event.TotalItems, @event.ElapsedTime);

        return Task.CompletedTask;
    }

    private Task HandleStreamCompleted(StreamCompletedEvent @event)
    {
        _logger.LogInformation(
            "Stream {StreamId} completed: {TotalItems} items in {TotalTime:g}",
            @event.StreamId, @event.TotalItems, @event.TotalTime);

        var metrics = _cache.Get<StreamingMetrics>($"stream:{@event.StreamId}");
        if (metrics != null)
        {
            metrics.IsCompleted = true;
            metrics.TotalItems = @event.TotalItems;
            metrics.ElapsedTime = @event.TotalTime;
            metrics.CompletedAt = DateTime.UtcNow;

            _cache.Set($"stream:{@event.StreamId}", metrics,
                TimeSpan.FromMinutes(30)); // Keep completed metrics for 30 minutes
        }

        return Task.CompletedTask;
    }

    private Task HandleStreamError(StreamErrorEvent @event)
    {
        _logger.LogError(@event.Error,
            "Stream {StreamId} error after processing {ItemsProcessed} items: {ErrorMessage}",
            @event.StreamId, @event.ItemsProcessedBeforeError, @event.Error.Message);

        var metrics = _cache.Get<StreamingMetrics>($"stream:{@event.StreamId}");
        if (metrics != null)
        {
            metrics.HasError = true;
            metrics.ErrorMessage = @event.Error.Message;
            metrics.ItemsProcessed = @event.ItemsProcessedBeforeError;

            _cache.Set($"stream:{@event.StreamId}", metrics,
                TimeSpan.FromMinutes(30));
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        foreach (var subscription in _subscriptions)
        {
            subscription.Dispose();
        }
    }

    private class StreamingMetrics
    {
        public string StreamId { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public int BatchSize { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? CompletedAt { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public int ItemsProcessed { get; set; }
        public int TotalItems { get; set; }
        public double ItemsPerSecond { get; set; }
        public bool IsCompleted { get; set; }
        public bool HasError { get; set; }
        public string? ErrorMessage { get; set; }
    }
}