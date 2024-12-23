using Microsoft.AspNetCore.Mvc;

namespace AsyncEnumerableApi.Infrastructure.AsyncEnumerable;

public class StreamingMiddleware
{
    private readonly RequestDelegate _next;

    public StreamingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            await _next(context);
            return;
        }

        var streamingAttribute = endpoint.Metadata.GetMetadata<StreamingAttribute>();
        if (streamingAttribute == null)
        {
            await _next(context);
            return;
        }

        // Store streaming configuration in HttpContext items for later use
        context.Items["StreamingBatchSize"] = streamingAttribute.BatchSize;
        context.Items["StreamingDelayMilliseconds"] = streamingAttribute.DelayMilliseconds;

        await _next(context);
    }
}