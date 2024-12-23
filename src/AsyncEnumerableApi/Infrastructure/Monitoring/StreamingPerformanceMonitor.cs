using System.Collections.Concurrent;
using System.Diagnostics;

namespace AsyncEnumerableApi.Infrastructure.Monitoring;

public class StreamingPerformanceMonitor
{
    private readonly ConcurrentDictionary<string, StreamMetrics> _activeStreams = new();
    private readonly ILogger<StreamingPerformanceMonitor> _logger;

    public StreamingPerformanceMonitor(ILogger<StreamingPerformanceMonitor> logger)
    {
        _logger = logger;
    }

    public string StartStream(string description)
    {
        var streamId = Guid.NewGuid().ToString();
        var metrics = new StreamMetrics
        {
            StreamId = streamId,
            Description = description,
            StartTime = DateTime.UtcNow,
            Stopwatch = Stopwatch.StartNew()
        };

        _activeStreams.TryAdd(streamId, metrics);
        _logger.LogInformation("Started stream {StreamId}: {Description}", streamId, description);

        return streamId;
    }

    public void UpdateProgress(string streamId, int itemsProcessed, long memoryUsed)
    {
        if (_activeStreams.TryGetValue(streamId, out var metrics))
        {
            metrics.ItemsProcessed = itemsProcessed;
            metrics.CurrentMemoryUsage = memoryUsed;
            metrics.ProcessingRate = itemsProcessed / metrics.Stopwatch.Elapsed.TotalSeconds;

            if (itemsProcessed % 1000 == 0) // Log every 1000 items
            {
                _logger.LogInformation(
                    "Stream {StreamId}: Processed {ItemCount} items at {Rate:F1} items/sec, Memory: {Memory:F2} MB",
                    streamId,
                    itemsProcessed,
                    metrics.ProcessingRate,
                    memoryUsed / (1024.0 * 1024.0));
            }
        }
    }

    public void CompleteStream(string streamId)
    {
        if (_activeStreams.TryRemove(streamId, out var metrics))
        {
            metrics.Stopwatch.Stop();
            metrics.EndTime = DateTime.UtcNow;

            _logger.LogInformation(
                "Completed stream {StreamId}: {ItemCount} items in {Duration:g}, Average rate: {Rate:F1} items/sec",
                streamId,
                metrics.ItemsProcessed,
                metrics.Stopwatch.Elapsed,
                metrics.ProcessingRate);
        }
    }

    public void ErrorStream(string streamId, Exception error)
    {
        if (_activeStreams.TryRemove(streamId, out var metrics))
        {
            metrics.Stopwatch.Stop();
            metrics.EndTime = DateTime.UtcNow;
            metrics.Error = error;

            _logger.LogError(error,
                "Error in stream {StreamId} after processing {ItemCount} items: {ErrorMessage}",
                streamId,
                metrics.ItemsProcessed,
                error.Message);
        }
    }

    public IReadOnlyList<StreamMetrics> GetActiveStreams()
    {
        return _activeStreams.Values.ToList();
    }

    public class StreamMetrics
    {
        public string StreamId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public Stopwatch Stopwatch { get; set; } = new();
        public int ItemsProcessed { get; set; }
        public double ProcessingRate { get; set; }
        public long CurrentMemoryUsage { get; set; }
        public Exception? Error { get; set; }

        public TimeSpan Duration => EndTime.HasValue 
            ? EndTime.Value - StartTime 
            : DateTime.UtcNow - StartTime;
    }
}