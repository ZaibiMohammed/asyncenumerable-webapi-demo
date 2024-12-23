using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AsyncEnumerableApi.Infrastructure.Swagger;

public class StreamingDocumentationFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // Add common parameters documentation
        var batchSizeParameter = new OpenApiParameter
        {
            Name = "batchSize",
            In = ParameterLocation.Query,
            Description = "Number of items to process in each batch",
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "integer",
                Default = new Microsoft.OpenApi.Any.OpenApiInteger(100),
                Minimum = 1,
                Maximum = 1000
            }
        };

        var delayParameter = new OpenApiParameter
        {
            Name = "delayMs",
            In = ParameterLocation.Query,
            Description = "Delay in milliseconds between batches",
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "integer",
                Default = new Microsoft.OpenApi.Any.OpenApiInteger(100),
                Minimum = 0,
                Maximum = 5000
            }
        };

        // Add common parameters to all operations that have the Streaming tag
        foreach (var path in swaggerDoc.Paths)
        {
            foreach (var operation in path.Value.Operations)
            {
                if (operation.Value.Tags.Any(t => t.Name == "Streaming"))
                {
                    if (!operation.Value.Parameters.Any(p => p.Name == "batchSize"))
                    {
                        operation.Value.Parameters.Add(batchSizeParameter);
                    }
                    if (!operation.Value.Parameters.Any(p => p.Name == "delayMs"))
                    {
                        operation.Value.Parameters.Add(delayParameter);
                    }
                }
            }
        }

        // Add custom schemas
        swaggerDoc.Components.Schemas.Add("StreamingOptions", new OpenApiSchema
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                { "batchSize", new OpenApiSchema { Type = "integer", Default = new Microsoft.OpenApi.Any.OpenApiInteger(100) } },
                { "maxBufferSize", new OpenApiSchema { Type = "integer", Default = new Microsoft.OpenApi.Any.OpenApiInteger(1000) } },
                { "delayBetweenItems", new OpenApiSchema { Type = "integer", Default = new Microsoft.OpenApi.Any.OpenApiInteger(0) } },
                { "enableBackpressure", new OpenApiSchema { Type = "boolean", Default = new Microsoft.OpenApi.Any.OpenApiBoolean(true) } }
            }
        });
    }
}