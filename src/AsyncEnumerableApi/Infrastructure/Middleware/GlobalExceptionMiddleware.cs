using System.Net;
using System.Text.Json;

namespace AsyncEnumerableApi.Infrastructure.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var result = new {
            Status = "Error",
            Message = _environment.IsDevelopment() ? 
                exception.Message : "An internal server error occurred.",
            Details = _environment.IsDevelopment() ? 
                exception.StackTrace : null
        };

        switch (exception)
        {
            case OperationCanceledException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                result = new { Status = "Error", Message = "The operation was canceled." };
                break;

            case InvalidOperationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        return response.WriteAsync(JsonSerializer.Serialize(result));
    }
}