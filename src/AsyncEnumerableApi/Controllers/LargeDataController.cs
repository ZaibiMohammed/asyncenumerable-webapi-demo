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
        // Generate large dataset
        var products = LargeDataGenerator.GenerateLargeDataset(totalItems);

        // Apply filters
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

        await foreach (var product in products.ToAsyncEnumerable())
        {
            yield return product;
            if (delayMs > 0)
            {
                await Task.Delay(delayMs, cancellationToken);
            }
        }
    }

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

        await foreach (var product in products.ToAsyncEnumerable())
        {
            if (product.Rating >= 4.0) // Filter for high-rated products
            {
                // Transform the product
                var qualityScore = product.Rating * Math.Log10(product.ReviewCount + 1);
                yield return product with
                {
                    Name = $"{product.Name} (Quality Score: {qualityScore:F1})"
                };
            }

            // Add delay for controlled streaming
            await Task.Delay(100, cancellationToken);
        }
    }

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