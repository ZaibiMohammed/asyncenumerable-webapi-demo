# AsyncEnumerable Web API Demo

This project demonstrates the use of IAsyncEnumerable in a .NET 8 Web API for efficient data streaming, along with an Angular 17 client that showcases real-time data consumption.

## Features

- Asynchronous data streaming using IAsyncEnumerable
- Real-time data loading with progress indication
- Filtering and pagination support
- Clean architecture with dependency injection
- Angular 17 client with Bootstrap UI
- Product catalog demo with 1000+ sample products

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

### Categories API

```http
GET /api/products/categories
```

Returns available product categories.

## Implementation Details

### Backend (.NET)
- Uses IAsyncEnumerable for efficient streaming
- Implements custom streaming middleware
- Includes sample data generation
- Supports filtering and pagination
- CORS configuration for local development

### Frontend (Angular)
- Real-time data loading
- Progress indication
- Filter panel for category and price range
- Responsive Bootstrap UI
- Error handling and loading states

## License

This project is licensed under the MIT License