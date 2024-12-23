using System.Diagnostics;

namespace AsyncEnumerableApi.Infrastructure.Monitoring;

public class StreamingMonitorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly StreamingMetricsCollector _metricsCollector;
    private readonly ILogger<StreamingMonitorMiddleware> _logger;

    public StreamingMonitorMiddleware(
        RequestDelegate next,
        StreamingMetricsCollector metricsCollector,
        ILogger<StreamingMonitorMiddleware> logger)
    {
        _next = next;
        _metricsCollector = metricsCollector;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/api/products/stream"))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var itemsStreamed = 0L;
        var batchesStreamed = 0L;
        var startMemory = GC.GetTotalMemory(false);

        context.Response.OnStarting(() =>
        {
            var endMemory = GC.GetTotalMemory(false);
            var metrics = new StreamingMetrics
            {
                EndpointName = context.Request.Path,
                ItemsStreamed = itemsStreamed,
                BatchesStreamed = batchesStreamed,
                AverageBatchSize = batchesStreamed > 0 ? (double)itemsStreamed / batchesStreamed : 0,
                TotalStreamingTime = stopwatch.Elapsed,
                ItemsPerSecond = stopwatch.Elapsed.TotalSeconds > 0 
                    ? itemsStreamed / stopwatch.Elapsed.TotalSeconds 
                    : 0,
                MemoryUsedBytes = endMemory - startMemory,
                Timestamp = DateTime.UtcNow
            };

            _metricsCollector.AddMetrics(metrics);
            _logger.LogInformation(
                "Streaming completed: {ItemsStreamed} items in {Elapsed:n2}s ({ItemsPerSecond:n1} items/s)",
                itemsStreamed,
                stopwatch.Elapsed.TotalSeconds,
                metrics.ItemsPerSecond);

            return Task.CompletedTask;
        });

        var originalStream = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        await _next(context);

        memoryStream.Position = 0;
        var responseContent = await new StreamReader(memoryStream).ReadToEndAsync();
        itemsStreamed = responseContent.Count(c => c == '\n');
        batchesStreamed = (itemsStreamed + 99) / 100; // Assuming batch size of 100

        memoryStream.Position = 0;
        await memoryStream.CopyToAsync(originalStream);
        context.Response.Body = originalStream;
    }
}