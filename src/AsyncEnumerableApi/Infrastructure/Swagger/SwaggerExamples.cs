using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AsyncEnumerableApi.Infrastructure.Swagger;

public class SwaggerExamples : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        AddStreamingExample(swaggerDoc);
    }

    private void AddStreamingExample(OpenApiDocument document)
    {
        var basicStreamPath = "/api/StreamingDemo/basic-stream";
        if (document.Paths.ContainsKey(basicStreamPath))
        {
            var operation = document.Paths[basicStreamPath].Operations[OperationType.Get];
            operation.RequestBody = new OpenApiRequestBody
            {
                Description = "Streaming request example",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Example = new Microsoft.OpenApi.Any.OpenApiObject
                        {
                            ["category"] = new Microsoft.OpenApi.Any.OpenApiString("Electronics"),
                            ["batchSize"] = new Microsoft.OpenApi.Any.OpenApiInteger(50)
                        }
                    }
                }
            };
        }
    }
}