namespace AsyncEnumerableApi.Infrastructure.Streaming;

public class StreamingOptions
{
    public int BatchSize { get; set; } = 100;
    public int MaxBufferSize { get; set; } = 1000;
    public int DelayBetweenItems { get; set; } = 0;
    public int ProgressUpdateInterval { get; set; } = 100;
    public int? ExpectedItemCount { get; set; }
    public bool EnableBackpressure { get; set; } = true;
    public TimeSpan? Timeout { get; set; }

    public static StreamingOptions Default => new();

    public StreamingOptions WithBatchSize(int batchSize)
    {
        BatchSize = batchSize;
        return this;
    }

    public StreamingOptions WithDelay(int milliseconds)
    {
        DelayBetweenItems = milliseconds;
        return this;
    }

    public StreamingOptions WithTimeout(TimeSpan timeout)
    {
        Timeout = timeout;
        return this;
    }

    public StreamingOptions WithExpectedCount(int count)
    {
        ExpectedItemCount = count;
        return this;
    }
}