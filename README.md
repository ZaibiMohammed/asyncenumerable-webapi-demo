# AsyncEnumerable Web API Demo

This project demonstrates advanced streaming capabilities in a .NET 8 Web API using IAsyncEnumerable with enhanced features including event bus, channels, and monitoring.

## Features

### Streaming Infrastructure
- Event-based streaming with monitoring
- Channel-based backpressure handling
- Configurable batching and rate limiting
- Progress tracking and metrics collection
- Error handling and retry mechanisms

### Streaming Operations
- Filtering and transformation
- Chunking and batching
- Progress reporting
- Rate limiting
- Backpressure handling
- Timeout and retry logic

### Monitoring and Events
- Stream start/progress/completion events
- Performance metrics collection
- Real-time progress tracking
- Error monitoring and reporting

## Getting Started

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022 or VS Code

### Running the Application
1. Clone the repository
2. Navigate to the API project:
```bash
cd src/AsyncEnumerableApi
dotnet run
```

## Usage Examples

### Basic Streaming
```csharp
[HttpGet("stream")]
public async IAsyncEnumerable<Product> GetProductsStream(
    CancellationToken cancellationToken)
{
    var products = await _repository.GetProducts();
    var options = new StreamingOptions()
        .WithBatchSize(50)
        .WithDelay(100);

    await foreach (var product in products.ToStreamingEnumerable(
        _eventBus, options, cancellationToken))
    {
        yield return product;
    }
}
```

### Advanced Features
```csharp
await foreach (var product in products
    .ToStreamingEnumerable(_eventBus)
    .WithFilter(async p => p.Price <= maxPrice)
    .WithTransform(async p => {
        p.Name = p.Name.ToUpper();
        return p;
    })
    .Chunk(50)
    .WithProgressReporting(count => 
        _logger.LogInformation("Processed {Count} items", count))
    .WithThrottling(100)
    .WithTimeout(TimeSpan.FromMinutes(5))
    .WithRetry(maxRetries: 3))
{
    yield return product;
}
```

## Architecture

### Event Bus
- InMemoryEventBus for event handling
- Stream lifecycle events (Start/Progress/Complete/Error)
- Subscription-based event monitoring

### Streaming Channel
- Channel-based streaming implementation
- Backpressure handling
- Configurable buffer sizes
- Rate limiting support

### Streaming Options
- Batch size configuration
- Delay settings
- Progress update intervals
- Timeout configuration
- Backpressure settings

## Monitoring

### Events
- StreamStartedEvent
- StreamProgressEvent
- StreamCompletedEvent
- StreamErrorEvent

### Metrics
- Items processed
- Processing rate
- Memory usage
- Error tracking

## Error Handling
- Retry mechanisms
- Timeout handling
- Error events
- Global exception handling

## Best Practices
1. Use appropriate batch sizes for your data
2. Configure reasonable timeouts
3. Implement proper error handling
4. Monitor stream progress
5. Handle backpressure appropriately

## License
This project is licensed under the MIT License