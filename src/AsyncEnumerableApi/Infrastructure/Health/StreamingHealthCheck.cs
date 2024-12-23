using AsyncEnumerableApi.Infrastructure.Monitoring;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AsyncEnumerableApi.Infrastructure.Health;

public class StreamingHealthCheck : IHealthCheck
{
    private readonly StreamingMetricsCollector _metricsCollector;

    public StreamingHealthCheck(StreamingMetricsCollector metricsCollector)
    {
        _metricsCollector = metricsCollector;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var metrics = _metricsCollector.GetMetrics();
        if (!metrics.Any())
        {
            return Task.FromResult(HealthCheckResult.Healthy("No streaming activity yet"));
        }

        var latestMetrics = metrics.LastOrDefault();
        if (latestMetrics == null)
        {
            return Task.FromResult(HealthCheckResult.Healthy());
        }

        // Check if streaming performance is within acceptable ranges
        var isHealthy = true;
        var data = new Dictionary<string, object>
        {
            { "LastStreamTime", latestMetrics.Timestamp },
            { "ItemsPerSecond", latestMetrics.ItemsPerSecond },
            { "MemoryUsed", latestMetrics.MemoryUsedBytes }
        };

        // Add warning if streaming rate is too low
        if (latestMetrics.ItemsPerSecond < 100) // Threshold of 100 items/second
        {
            isHealthy = false;
            data.Add("Warning", "Low streaming rate detected");
        }

        // Add warning if memory usage is too high
        if (latestMetrics.MemoryUsedBytes > 100_000_000) // 100MB threshold
        {
            isHealthy = false;
            data.Add("Warning", "High memory usage detected");
        }

        return Task.FromResult(isHealthy
            ? HealthCheckResult.Healthy("Streaming system is healthy", data)
            : HealthCheckResult.Degraded("Streaming system performance is degraded", null, data));
    }
}