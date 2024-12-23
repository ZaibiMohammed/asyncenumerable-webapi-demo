namespace AsyncEnumerableApi.Infrastructure.Monitoring;

public class StreamingMetrics
{
    public string EndpointName { get; set; } = string.Empty;
    public long ItemsStreamed { get; set; }
    public long BatchesStreamed { get; set; }
    public double AverageBatchSize { get; set; }
    public TimeSpan TotalStreamingTime { get; set; }
    public double ItemsPerSecond { get; set; }
    public long MemoryUsedBytes { get; set; }
    public DateTime Timestamp { get; set; }
}

public class StreamingMetricsCollector
{
    private readonly List<StreamingMetrics> _metrics = new();
    private readonly object _lock = new();

    public void AddMetrics(StreamingMetrics metrics)
    {
        lock (_lock)
        {
            _metrics.Add(metrics);
            // Keep only last 100 metrics
            if (_metrics.Count > 100)
            {
                _metrics.RemoveAt(0);
            }
        }
    }

    public IReadOnlyList<StreamingMetrics> GetMetrics()
    {
        lock (_lock)
        {
            return _metrics.ToList();
        }
    }
}