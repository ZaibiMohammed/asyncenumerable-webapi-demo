using System.Runtime.CompilerServices;

namespace AsyncEnumerableApi.Infrastructure.Streaming;

public static class StreamingExtensions
{
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(
        this IEnumerable<T> source,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var item in source)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return item;
        }
    }

    public static async IAsyncEnumerable<IReadOnlyList<T>> Chunk<T>(
        this IAsyncEnumerable<T> source,
        int chunkSize,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (chunkSize <= 0) throw new ArgumentException("Chunk size must be positive", nameof(chunkSize));

        var chunk = new List<T>(chunkSize);
        await foreach (var item in source.WithCancellation(cancellationToken))
        {
            chunk.Add(item);
            if (chunk.Count == chunkSize)
            {
                yield return chunk.ToList();
                chunk.Clear();
            }
        }

        if (chunk.Count > 0)
        {
            yield return chunk.ToList();
        }
    }

    public static async IAsyncEnumerable<T> WithProgressReporting<T>(
        this IAsyncEnumerable<T> source,
        Action<int> progressCallback,
        int reportInterval = 100,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var count = 0;
        await foreach (var item in source.WithCancellation(cancellationToken))
        {
            yield return item;
            count++;

            if (count % reportInterval == 0)
            {
                progressCallback(count);
            }
        }
    }

    public static async IAsyncEnumerable<T> WithThrottling<T>(
        this IAsyncEnumerable<T> source,
        int itemsPerSecond,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (itemsPerSecond <= 0) throw new ArgumentException("Items per second must be positive", nameof(itemsPerSecond));

        var delayMs = 1000 / itemsPerSecond;
        await foreach (var item in source.WithCancellation(cancellationToken))
        {
            yield return item;
            await Task.Delay(delayMs, cancellationToken);
        }
    }

    public static async IAsyncEnumerable<T> WithBackpressure<T>(
        this IAsyncEnumerable<T> source,
        int maxBufferSize,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var channel = Channel.CreateBounded<T>(new BoundedChannelOptions(maxBufferSize)
        {
            FullMode = BoundedChannelFullMode.Wait
        });

        var writeTask = Task.Run(async () =>
        {
            try
            {
                await foreach (var item in source.WithCancellation(cancellationToken))
                {
                    await channel.Writer.WriteAsync(item, cancellationToken);
                }
                channel.Writer.Complete();
            }
            catch (Exception ex)
            {
                channel.Writer.Complete(ex);
                throw;
            }
        }, cancellationToken);

        await foreach (var item in channel.Reader.ReadAllAsync(cancellationToken))
        {
            yield return item;
        }

        await writeTask;
    }
}