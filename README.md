# AsyncEnumerable Web API Demo

This project demonstrates the use of IAsyncEnumerable in a .NET 8 Web API for efficient data streaming, along with an Angular 17 client that showcases real-time data consumption and monitoring.

## Features

### Backend (.NET 8)
- Asynchronous data streaming using IAsyncEnumerable
- Health monitoring system
- Performance metrics collection
- Global error handling
- Retry policies for resilience
- Clean architecture with dependency injection
- Product catalog demo with 1000+ sample products

### Frontend (Angular 17)
- Real-time data loading with progress indication
- Health monitoring dashboard
- Performance metrics visualization
- Error notification system
- Filtering and pagination support
- Responsive Bootstrap UI

## Project Structure

- `src/AsyncEnumerableApi`: .NET 8 Web API project
- `client`: Angular 17 client application

## Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js and npm
- Angular CLI (`npm install -g @angular/cli`)

### Running the API

1. Navigate to the API project directory:
```bash
cd src/AsyncEnumerableApi
```

2. Run the API:
```bash
dotnet run
```

The API will be available at `https://localhost:7001`

### Running the Angular Client

1. Navigate to the client directory:
```bash
cd client
```

2. Install dependencies:
```bash
npm install
```

3. Start the development server:
```bash
ng serve
```

The client will be available at `http://localhost:4200`

## API Endpoints

### Products API

```http
GET /api/products/stream?pageSize=20&category=Electronics&minPrice=100&maxPrice=1000
```

Parameters:
- `pageSize`: Number of items per batch (default: 20)
- `category`: Filter by product category
- `minPrice`: Minimum price filter
- `maxPrice`: Maximum price filter

### Health Check API

```http
GET /health
```

Returns the system health status including:
- Overall system health
- Component status
- Performance metrics
- Memory usage

## Implementation Details

### Backend Features
- Uses IAsyncEnumerable for efficient streaming
- Implements custom streaming middleware
- Includes sample data generation
- Health monitoring system
- Performance metrics collection
- Global error handling
- CORS configuration

### Frontend Features
- Real-time data loading
- Health status monitoring
- Performance visualization
- Error notification system
- Progress indication
- Responsive design

## Monitoring and Error Handling

### Health Monitoring
- Real-time system health updates
- Component status tracking
- Performance metrics visualization
- Memory usage monitoring

### Error Handling
- Global exception middleware
- Toast notifications
- Retry policies
- Error logging

## License

This project is licensed under the MIT License