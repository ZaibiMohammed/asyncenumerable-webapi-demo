# AsyncEnumerable Web API Demo

This project demonstrates the use of IAsyncEnumerable in a .NET 8 Web API for efficient data streaming.

## Features

- Asynchronous data streaming using IAsyncEnumerable
- Configurable batch sizes and delays
- Cancellation support
- Clean architecture with dependency injection
- Swagger UI integration

## Getting Started

### Prerequisites

- .NET 8 SDK
- Visual Studio 2022 or VS Code

### Running the Application

1. Clone the repository
2. Navigate to the project directory
3. Run the application:
   ```bash
   dotnet restore
   dotnet run --project src/AsyncEnumerableApi/AsyncEnumerableApi.csproj
   ```
4. Open your browser and navigate to `https://localhost:7001/swagger` to view the Swagger UI

## API Usage

The API exposes an endpoint that streams data items:

```http
GET /api/streaming/data?totalItems=1000&batchSize=100&delayMs=100
```

Parameters:
- `totalItems`: Total number of items to generate (default: 1000)
- `batchSize`: Number of items per batch (default: 100)
- `delayMs`: Delay between batches in milliseconds (default: 100)

## Example Usage with curl

```bash
curl -N "https://localhost:7001/api/streaming/data?totalItems=500&batchSize=50&delayMs=100"
```

## Performance Considerations

- The API uses streaming to efficiently handle large datasets
- Memory usage is controlled through batch processing
- Configurable delays prevent overwhelming the client or network
- Cancellation support allows clients to stop the stream when needed

## Architecture

The project follows clean architecture principles:

- Controllers: Handle HTTP requests and responses
- Services: Contain business logic and data streaming implementation
- Models: Define the data structures

## License

This project is licensed under the MIT License