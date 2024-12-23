namespace AsyncEnumerableApi.Infrastructure.AsyncEnumerable;

[AttributeUsage(AttributeTargets.Method)]
public class StreamingAttribute : Attribute
{
    public int BatchSize { get; set; } = 100;
    public int DelayMilliseconds { get; set; } = 100;

    public StreamingAttribute()
    {
    }

    public StreamingAttribute(int batchSize, int delayMilliseconds)
    {
        BatchSize = batchSize;
        DelayMilliseconds = delayMilliseconds;
    }
}