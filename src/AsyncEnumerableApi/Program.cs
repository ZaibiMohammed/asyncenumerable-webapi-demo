using AsyncEnumerableApi.Infrastructure.EventBus;
using AsyncEnumerableApi.Infrastructure.Streaming;
using AsyncEnumerableApi.Infrastructure.Streaming.Handlers;
using AsyncEnumerableApi.Infrastructure.Swagger;
using AsyncEnumerableApi.Services;
using Microsoft.OpenApi.Models;
using System.Reflection;

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

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Streaming API Demo",
        Version = "v1",
        Description = "A Web API demonstrating advanced streaming capabilities using IAsyncEnumerable",
        Contact = new OpenApiContact
        {
            Name = "Your Name",
            Email = "your.email@example.com"
        }
    });

    // Add XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Add streaming operation filter
    c.OperationFilter<StreamingOperationFilter>();

    // Add custom documentation for common parameters
    c.DocumentFilter<StreamingDocumentationFilter>();
});

// Register services
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IEventBus, InMemoryEventBus>();
builder.Services.AddSingleton<StreamingEventHandler>();
builder.Services.AddSingleton<IProductService, ProductService>();

var app = builder.Build();

// Ensure the StreamingEventHandler is created to start handling events
app.Services.GetRequiredService<StreamingEventHandler>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Streaming API V1");
        c.EnableDeepLinking();
        c.DisplayRequestDuration();
    });
}

app.UseCors("AllowAngularDev");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();