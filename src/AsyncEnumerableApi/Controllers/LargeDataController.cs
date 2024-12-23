using AsyncEnumerableApi.Infrastructure.EventBus;
using AsyncEnumerableApi.Infrastructure.Streaming;
using AsyncEnumerableApi.Models;
using AsyncEnumerableApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AsyncEnumerableApi.Controllers;

/// <summary>
/// Controller for demonstrating streaming with large datasets
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
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
    /// Streams a large dataset of products with configurable size and filtering
    /// </summary>
    /// <param name="totalItems">Total number of items to generate (default: 10000)</param>
    /// <param name="category">Optional category filter</param>
    /// <param name="minPrice">Minimum price filter</param>
    /// <param name="maxPrice">Maximum price filter</param>
    /// <param name="batchSize">Number of items per batch (default: 1000)</param>
    /// <param name="delayMs">Delay between batches in milliseconds (default: 100)</param>
    /// <returns>A stream of products</returns>
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

        var options = new StreamingOptions()
            .WithBatchSize(batchSize)
            .WithDelay(delayMs)
            .WithExpectedCount(totalItems);

        await foreach (var product in products
            .ToStreamingEnumerable(_eventBus, options)
            .WithProgressReporting(count => 
                _logger.LogInformation("Streamed {Count} items", count))
            .WithCancellation(cancellationToken))
        {
            yield return product;
        }
    }

    /// <summary>
    /// Streams products with advanced features including transformation and analysis
    /// </summary>
    [HttpGet("advanced-stream")]
    public async IAsyncEnumerable<Product> GetAdvancedLargeDataStream(
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

        var options = new StreamingOptions()
            .WithBatchSize(batchSize)
            .WithDelay(100)
            .WithExpectedCount(totalItems);

        await foreach (var product in products
            .ToStreamingEnumerable(_eventBus, options)
            .WithFilter(async p => p.Rating >= 4.0) // Only high-rated products
            .WithTransform(async p =>
            {
                // Add a quality indicator based on rating and review count
                var qualityScore = p.Rating * Math.Log10(p.ReviewCount + 1);
                return p with
                {
                    Name = $"{p.Name} (Quality Score: {qualityScore:F1})"
                };
            })
            .WithProgressReporting(count => 
                _logger.LogInformation("Processed {Count} items", count))
            .WithThrottling(1000) // 1000 items per second
            .WithTimeout(TimeSpan.FromMinutes(5))
            .WithRetry(maxRetries: 3)
            .WithCancellation(cancellationToken))
        {
            yield return product;
        }
    }

    /// <summary>
    /// Gets available categories and their statistics
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