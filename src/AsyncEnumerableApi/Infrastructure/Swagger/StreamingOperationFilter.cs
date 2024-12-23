using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections;

namespace AsyncEnumerableApi.Infrastructure.Swagger;

public class StreamingOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var returnType = context.MethodInfo.ReturnType;
        
        if (IsAsyncEnumerable(returnType))
        {
            // Add description about streaming behavior
            operation.Description += "\n\nThis endpoint supports streaming data using IAsyncEnumerable.";
            operation.Description += "\nData will be streamed in chunks with configurable batch sizes and delays.";

            // Add response header for transfer encoding
            operation.Responses["200"].Headers.Add("Transfer-Encoding", new OpenApiHeader
            {
                Description = "Set to 'chunked' for streaming responses",
                Schema = new OpenApiSchema { Type = "string" }
            });

            // Add tags for streaming endpoints
            if (!operation.Tags.Any(t => t.Name == "Streaming"))
            {
                operation.Tags.Add(new OpenApiTag { Name = "Streaming" });
            }
        }
    }

    private bool IsAsyncEnumerable(Type type)
    {
        if (type == null) return false;

        return type.IsGenericType && 
               (type.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>) ||
                type.GetInterfaces().Any(i => i.IsGenericType && 
                    i.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>)));
    }
}