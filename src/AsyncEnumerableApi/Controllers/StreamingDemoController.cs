using AsyncEnumerableApi.Infrastructure.EventBus;
using AsyncEnumerableApi.Infrastructure.Streaming;
using AsyncEnumerableApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace AsyncEnumerableApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StreamingDemoController : ControllerBase
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<StreamingDemoController> _logger;

    public StreamingDemoController(
        IEventBus eventBus,
        ILogger<StreamingDemoController> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    [HttpGet("basic-stream")]
    [Streaming(BatchSize = 50, DelayMilliseconds = 100)]
    public async IAsyncEnumerable<DataItem> GetDataStream(
        [FromQuery] int totalItems = 1000,
        [FromQuery] int batchSize = 100,
        [FromQuery] int delayMs = 100,
        CancellationToken cancellationToken = default)
    {
        var data = Enumerable.Range(0, totalItems)
            .Select(id => new DataItem
            {
                Id = id,
                Name = $"Item {id}",
                Timestamp = DateTime.UtcNow,
                Value = (decimal)(new Random().NextDouble() * 1000)
            });

        await foreach (var item in data.ToAsyncEnumerable())
        {
            yield return item;
            await Task.Delay(delayMs, cancellationToken);
        }
    }

    [HttpGet("wrapped")]
    public async IAsyncEnumerable<DataItem> GetWrappedData(
        CancellationToken cancellationToken = default)
    {
        var batchSize = HttpContext.Items["StreamingBatchSize"] as int? ?? 100;
        var delayMs = HttpContext.Items["StreamingDelayMilliseconds"] as int? ?? 100;

        await foreach (var item in GetDataStream(
            totalItems: 1000,
            batchSize: batchSize,
            delayMs: delayMs,
            cancellationToken: cancellationToken))
        {
            yield return item;
        }
    }
}