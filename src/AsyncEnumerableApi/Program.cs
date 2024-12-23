using AsyncEnumerableApi.Infrastructure.AsyncEnumerable;
using AsyncEnumerableApi.Infrastructure.Health;
using AsyncEnumerableApi.Infrastructure.Middleware;
using AsyncEnumerableApi.Infrastructure.Monitoring;
using AsyncEnumerableApi.Services;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev",
        builder => builder
            .WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register health checks
builder.Services.AddHealthChecks()
    .AddCheck<StreamingHealthCheck>("streaming_health_check");

// Register services
builder.Services.AddSingleton<IDataStreamingService, DataStreamingService>();
builder.Services.AddSingleton<IProductService, ProductService>();
builder.Services.AddSingleton<StreamingMetricsCollector>();

var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngularDev");

// Add custom middleware
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<StreamingMonitorMiddleware>();
app.UseMiddleware<StreamingMiddleware>();

// Configure health check endpoint
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();