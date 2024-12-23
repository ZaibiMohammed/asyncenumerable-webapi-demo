using AsyncEnumerableApi.Infrastructure.AsyncEnumerable;
using AsyncEnumerableApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace AsyncEnumerableApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DemoController : ControllerBase
{
    private readonly ILogger<DemoController> _logger;

    public DemoController(ILogger<DemoController> logger)
    {
        _logger = logger;
    }

    [HttpGet("classic")]
    [Streaming(BatchSize = 50, DelayMilliseconds = 100)]
    public async Task<IEnumerable<DataItem>> GetClassicData()
    {
        // Simulate getting data from a database
        var items = Enumerable.Range(0, 1000)
            .Select(id => new DataItem
            {
                Id = id,
                Name = $"Item {id}",
                Timestamp = DateTime.UtcNow,
                Value = (decimal)(new Random().NextDouble() * 1000)
            });

        return items;
    }

    [HttpGet("wrapped")]
    public async IAsyncEnumerable<DataItem> GetWrappedData(
        CancellationToken cancellationToken = default)
    {
        var batchSize = HttpContext.Items["StreamingBatchSize"] as int? ?? 100;
        var delayMs = HttpContext.Items["StreamingDelayMilliseconds"] as int? ?? 100;

        var data = await GetClassicData();
        await foreach (var item in data.ToAsyncEnumerable(
            batchSize, delayMs, cancellationToken))
        {
            yield return item;
        }
    }
}