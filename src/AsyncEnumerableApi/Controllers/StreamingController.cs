using AsyncEnumerableApi.Models;
using AsyncEnumerableApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AsyncEnumerableApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StreamingController : ControllerBase
{
    private readonly IDataStreamingService _dataStreamingService;
    private readonly ILogger<StreamingController> _logger;

    public StreamingController(
        IDataStreamingService dataStreamingService,
        ILogger<StreamingController> logger)
    {
        _dataStreamingService = dataStreamingService;
        _logger = logger;
    }

    /// <summary>
    /// Streams data items asynchronously
    /// </summary>
    /// <param name="totalItems">Total number of items to generate</param>
    /// <param name="batchSize">Number of items per batch</param>
    /// <param name="delayMs">Delay between batches in milliseconds</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A stream of data items</returns>
    [HttpGet("data")]
    public IAsyncEnumerable<DataItem> StreamData(
        [FromQuery] int totalItems = 1000,
        [FromQuery] int batchSize = 100,
        [FromQuery] int delayMs = 100,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Starting data stream for {TotalItems} items in batches of {BatchSize} with {DelayMs}ms delay",
            totalItems, batchSize, delayMs);

        return _dataStreamingService.StreamDataAsync(totalItems, batchSize, delayMs, cancellationToken);
    }
}