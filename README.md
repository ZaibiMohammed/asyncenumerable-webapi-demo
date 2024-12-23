# AsyncEnumerable Web API Demo

A comprehensive demonstration of streaming capabilities in .NET 8 using IAsyncEnumerable, showcasing large dataset handling, performance monitoring, and real-time analytics.

## Features

### Streaming Capabilities
- Efficient large dataset streaming
- Configurable batch processing
- Progress monitoring and analytics
- Memory-efficient data handling

### Key Components
- AsyncEnumerable extensions for enhanced streaming
- Performance monitoring and metrics
- Real-time data analytics
- Swagger documentation

## Getting Started

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022 or VS Code

### Running the Application

```bash
# Clone the repository
git clone https://github.com/yourusername/asyncenumerable-webapi-demo.git
cd asyncenumerable-webapi-demo

# Run the API
dotnet run --project src/AsyncEnumerableApi/AsyncEnumerableApi.csproj
```

The API will be available at `https://localhost:7001`

## API Endpoints

### Basic Streaming
```http
GET /api/LargeData/stream?totalItems=10000&batchSize=1000
```

Parameters:
- `totalItems`: Number of items to generate (default: 10000)
- `batchSize`: Items per batch (default: 1000)
- `delayMs`: Delay between batches in milliseconds
- `category`: Filter by category
- `minPrice`: Minimum price filter
- `maxPrice`: Maximum price filter

### Advanced Streaming
```http
GET /api/LargeData/advanced-stream
```

Features:
- High-rated product filtering
- Quality score calculation
- Progress monitoring
- Batch processing

### Category Analytics
```http
GET /api/LargeData/categories
```

Provides category-wise statistics including:
- Product counts
- Average prices
- Total stock
- Average ratings

## Implementation Details

### AsyncEnumerable Extensions
```csharp
// Stream with delay between items
await foreach (var item in source.WithDelay(100))

// Stream with batch processing
await foreach (var item in source.WithBatchDelay(1000, 100))

// Stream with progress reporting
await foreach (var item in source.WithProgress(count => 
    Console.WriteLine($"Processed {count} items")))
```

### Performance Monitoring
- Real-time streaming metrics
- Memory usage tracking
- Processing rate monitoring
- Error handling

## Best Practices

1. **Memory Management**
   - Use appropriate batch sizes
   - Monitor memory usage
   - Implement backpressure when needed

2. **Error Handling**
   - Implement retry logic
   - Handle cancellation properly
   - Log errors and progress

3. **Performance**
   - Configure batch sizes based on data size
   - Adjust delays based on client capabilities
   - Monitor streaming metrics

## License

This project is licensed under the MIT License
