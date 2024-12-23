using System.Collections.Concurrent;

namespace AsyncEnumerableApi.Services;

public class StreamAnalysisService
{
    private readonly ConcurrentDictionary<string, StreamAnalytics> _analytics = new();
    private readonly ILogger<StreamAnalysisService> _logger;

    public StreamAnalysisService(ILogger<StreamAnalysisService> logger)
    {
        _logger = logger;
    }

    public string StartAnalysis(string description)
    {
        var analysisId = Guid.NewGuid().ToString();
        var analytics = new StreamAnalytics(description);
        _analytics.TryAdd(analysisId, analytics);
        return analysisId;
    }

    public void ProcessItem<T>(string analysisId, T item)
    {
        if (_analytics.TryGetValue(analysisId, out var analytics))
        {
            analytics.ProcessItem(item);
        }
    }

    public StreamAnalytics? GetAnalytics(string analysisId)
    {
        return _analytics.TryGetValue(analysisId, out var analytics) ? analytics : null;
    }

    public void CompleteAnalysis(string analysisId)
    {
        if (_analytics.TryRemove(analysisId, out var analytics))
        {
            analytics.Complete();
            _logger.LogInformation(
                "Analysis {AnalysisId} completed: {ItemCount} items processed",
                analysisId,
                analytics.ItemCount);
        }
    }

    public class StreamAnalytics
    {
        private readonly ConcurrentDictionary<string, long> _categoryCounts = new();
        private readonly ConcurrentDictionary<string, RunningStatistics> _numericStats = new();
        private readonly List<TimeSpan> _processingIntervals = new();
        private DateTime _lastProcessedTime = DateTime.UtcNow;

        public string Description { get; }
        public int ItemCount { get; private set; }
        public DateTime StartTime { get; }
        public DateTime? EndTime { get; private set; }

        public StreamAnalytics(string description)
        {
            Description = description;
            StartTime = DateTime.UtcNow;
        }

        public void ProcessItem<T>(T item)
        {
            ItemCount++;

            var now = DateTime.UtcNow;
            var interval = now - _lastProcessedTime;
            _processingIntervals.Add(interval);
            _lastProcessedTime = now;

            // Update category counts for string properties
            foreach (var prop in typeof(T).GetProperties())
            {
                if (prop.PropertyType == typeof(string))
                {
                    var value = prop.GetValue(item)?.ToString() ?? "null";
                    _categoryCounts.AddOrUpdate(
                        $"{prop.Name}:{value}",
                        1,
                        (_, count) => count + 1);
                }
                else if (prop.PropertyType == typeof(decimal) || 
                         prop.PropertyType == typeof(double) || 
                         prop.PropertyType == typeof(int))
                {
                    var value = Convert.ToDouble(prop.GetValue(item));
                    _numericStats.GetOrAdd(prop.Name, _ => new RunningStatistics())
                                .Update(value);
                }
            }
        }

        public void Complete()
        {
            EndTime = DateTime.UtcNow;
        }

        public IReadOnlyDictionary<string, long> GetCategoryCounts()
        {
            return _categoryCounts;
        }

        public IReadOnlyDictionary<string, StatisticsSummary> GetNumericStatistics()
        {
            return _numericStats.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.GetSummary());
        }

        public ProcessingStatistics GetProcessingStatistics()
        {
            return new ProcessingStatistics
            {
                TotalDuration = (EndTime ?? DateTime.UtcNow) - StartTime,
                ItemsPerSecond = ItemCount / (EndTime ?? DateTime.UtcNow).Subtract(StartTime).TotalSeconds,
                AverageInterval = TimeSpan.FromTicks((long)_processingIntervals.Average(i => i.Ticks)),
                MinInterval = _processingIntervals.Min(),
                MaxInterval = _processingIntervals.Max()
            };
        }
    }

    public class RunningStatistics
    {
        private double _count;
        private double _mean;
        private double _m2;
        private double _min = double.MaxValue;
        private double _max = double.MinValue;

        public void Update(double value)
        {
            _count++;
            var delta = value - _mean;
            _mean += delta / _count;
            var delta2 = value - _mean;
            _m2 += delta * delta2;

            _min = Math.Min(_min, value);
            _max = Math.Max(_max, value);
        }

        public StatisticsSummary GetSummary()
        {
            return new StatisticsSummary
            {
                Count = _count,
                Mean = _mean,
                Variance = _count > 1 ? _m2 / (_count - 1) : 0,
                Min = _min,
                Max = _max
            };
        }
    }

    public record StatisticsSummary
    {
        public double Count { get; init; }
        public double Mean { get; init; }
        public double Variance { get; init; }
        public double StandardDeviation => Math.Sqrt(Variance);
        public double Min { get; init; }
        public double Max { get; init; }
    }

    public record ProcessingStatistics
    {
        public TimeSpan TotalDuration { get; init; }
        public double ItemsPerSecond { get; init; }
        public TimeSpan AverageInterval { get; init; }
        public TimeSpan MinInterval { get; init; }
        public TimeSpan MaxInterval { get; init; }
    }
}