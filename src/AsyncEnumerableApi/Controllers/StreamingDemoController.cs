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
    public async IAsyncEnumerable<Product> GetBasicStream(
        [FromQuery] string? category,
        [FromQuery] int batchSize = 100,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var products = GetSampleProducts(category);
        var options = new StreamingOptions()
            .WithBatchSize(batchSize)
            .WithDelay(100)
            .WithExpectedCount(products.Count());

        await foreach (var product in products.ToStreamingEnumerable(
            _eventBus, options, cancellationToken))
        {
            yield return product;
        }
    }

    [HttpGet("advanced-stream")]
    public async IAsyncEnumerable<Product> GetAdvancedStream(
        [FromQuery] string? category,
        [FromQuery] int batchSize = 100,
        [FromQuery] int? maxPrice = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var products = GetSampleProducts(category);
        
        await foreach (var product in products
            .ToStreamingEnumerable(_eventBus)
            .WithFilter(async p => maxPrice == null || p.Price <= maxPrice)
            .WithTransform(async p =>
            {
                p.Name = p.Name.ToUpper(); // Example transformation
                return p;
            })
            .Chunk(batchSize)
            .WithProgressReporting(count => 
                _logger.LogInformation("Processed {Count} items", count))
            .WithThrottling(100) // 100 items per second
            .WithTimeout(TimeSpan.FromMinutes(5))
            .WithRetry(maxRetries: 3)
            .WithCancellation(cancellationToken))
        {
            foreach (var product in product) // product is a chunk
            {
                yield return product;
            }
        }
    }

    private IEnumerable<Product> GetSampleProducts(string? category)
    {
        var products = SeedData.GenerateProducts(1000);
        if (!string.IsNullOrEmpty(category))
        {
            products = products.Where(p => p.Category == category);
        }
        return products;
    }
}