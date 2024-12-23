using System.Net;

namespace AsyncEnumerableApi.Infrastructure.Resilience;

public class RetryPolicyHandler : DelegatingHandler
{
    private readonly ILogger<RetryPolicyHandler> _logger;
    private readonly int _maxRetries;
    private readonly int _initialRetryDelayMs;

    public RetryPolicyHandler(
        ILogger<RetryPolicyHandler> logger,
        int maxRetries = 3,
        int initialRetryDelayMs = 100)
    {
        _logger = logger;
        _maxRetries = maxRetries;
        _initialRetryDelayMs = initialRetryDelayMs;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage? response = null;
        var retryCount = 0;
        var delay = _initialRetryDelayMs;

        while (retryCount < _maxRetries)
        {
            try
            {
                response = await base.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode || 
                    !IsTransientError(response.StatusCode))
                {
                    return response;
                }

                retryCount++;
                if (retryCount < _maxRetries)
                {
                    _logger.LogWarning(
                        "Request failed with {StatusCode}. Retrying in {Delay}ms. Attempt {RetryCount}/{MaxRetries}",
                        response.StatusCode, delay, retryCount, _maxRetries);

                    await Task.Delay(delay, cancellationToken);
                    delay *= 2; // Exponential backoff
                }
            }
            catch (HttpRequestException ex)
            {
                retryCount++;
                if (retryCount == _maxRetries)
                {
                    throw;
                }

                _logger.LogWarning(
                    ex,
                    "Request failed with exception. Retrying in {Delay}ms. Attempt {RetryCount}/{MaxRetries}",
                    delay, retryCount, _maxRetries);

                await Task.Delay(delay, cancellationToken);
                delay *= 2;
            }
        }

        return response ?? throw new HttpRequestException("All retry attempts failed");
    }

    private bool IsTransientError(HttpStatusCode statusCode)
    {
        return statusCode == HttpStatusCode.RequestTimeout ||
               statusCode == HttpStatusCode.BadGateway ||
               statusCode == HttpStatusCode.ServiceUnavailable ||
               statusCode == HttpStatusCode.GatewayTimeout ||
               (int)statusCode == 429; // Too Many Requests
    }
}