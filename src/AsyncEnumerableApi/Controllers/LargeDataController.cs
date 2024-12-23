using AsyncEnumerableApi.Infrastructure.AsyncEnumerable;
using AsyncEnumerableApi.Infrastructure.EventBus;
using AsyncEnumerableApi.Infrastructure.Streaming;
using AsyncEnumerableApi.Models;
using AsyncEnumerableApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AsyncEnumerableApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LargeDataController : ControllerBase
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<LargeDataController> _logger;

    public LargeDataController(
        IEventBus eventBus,
        ILogger<LargeDataController> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    /// <summary>
    /// Streams a large dataset with filtering and batching
    /// </summary>
    [HttpGet("stream")]
    public async IAsyncEnumerable<Product> GetLargeDataStream(
        [FromQuery] int totalItems = 10000,
        [FromQuery] string? category = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] int batchSize = 1000,
        [FromQuery] int delayMs = 100,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var products = LargeDataGenerator.GenerateLargeDataset(totalItems);

        if (!string.IsNullOrEmpty(category))
        {
            products = products.Where(p => p.Category == category);
        }
        if (minPrice.HasValue)
        {
            products = products.Where(p => p.Price >= minPrice.Value);
        }
        if (maxPrice.HasValue)
        {
            products = products.Where(p => p.Price <= maxPrice.Value);
        }

        await foreach (var product in products.ToAsyncEnumerable()
            .WithBatchDelay(batchSize, delayMs)
            .WithProgress(count => _logger.LogInformation(
                "Processed {Count} items", count))
            .WithCancellation(cancellationToken))
        {
            yield return product;
        }
    }

    /// <summary>
    /// Advanced streaming with filtering, transformation and analytics
    /// </summary>
    [HttpGet("advanced-stream")]
    public async IAsyncEnumerable<Product> GetAdvancedStream(
        [FromQuery] int totalItems = 10000,
        [FromQuery] string? category = null,
        [FromQuery] int batchSize = 1000,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var products = LargeDataGenerator.GenerateLargeDataset(totalItems);

        if (!string.IsNullOrEmpty(category))
        {
            products = products.Where(p => p.Category == category);
        }

        await foreach (var product in products.ToAsyncEnumerable()
            .WithFilter(p => p.Rating >= 4.0)
            .WithBatchDelay(batchSize, 100)
            .WithProgress(count => _logger.LogInformation(
                "Processed {Count} high-rated items", count))
            .WithCancellation(cancellationToken))
        {
            var qualityScore = product.Rating * Math.Log10(product.ReviewCount + 1);
            yield return product with
            {
                Name = $"{product.Name} (Quality Score: {qualityScore:F1})"
            };
        }
    }

    /// <summary>
    /// Gets category statistics
    /// </summary>
    [HttpGet("categories")]
    public ActionResult<IEnumerable<object>> GetCategoryStats()
    {
        var products = LargeDataGenerator.GenerateLargeDataset(1000).ToList();
        var stats = products
            .GroupBy(p => p.Category)
            .Select(g => new
            {
                Category = g.Key,
                ProductCount = g.Count(),
                AveragePrice = g.Average(p => p.Price),
                TotalStock = g.Sum(p => p.Stock),
                AverageRating = g.Average(p => p.Rating)
            })
            .OrderByDescending(s => s.ProductCount);

        return Ok(stats);
    }
}