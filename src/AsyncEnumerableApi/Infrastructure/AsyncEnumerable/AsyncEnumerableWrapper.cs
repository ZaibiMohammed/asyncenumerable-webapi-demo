using System.Runtime.CompilerServices;

namespace AsyncEnumerableApi.Infrastructure.AsyncEnumerable;

public static class AsyncEnumerableWrapper
{
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(
        this IEnumerable<T> source,
        int batchSize = 100,
        int delayMilliseconds = 100,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var buffer = new List<T>();

        foreach (var item in source)
        {
            buffer.Add(item);

            if (buffer.Count >= batchSize)
            {
                foreach (var bufferedItem in buffer)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return bufferedItem;
                }

                buffer.Clear();

                if (delayMilliseconds > 0)
                {
                    await Task.Delay(delayMilliseconds, cancellationToken);
                }
            }
        }

        // Return any remaining items
        foreach (var item in buffer)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return item;
        }
    }

    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(
        this Task<IEnumerable<T>> sourceTask,
        int batchSize = 100,
        int delayMilliseconds = 100,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var source = await sourceTask;
        await foreach (var item in source.ToAsyncEnumerable(batchSize, delayMilliseconds, cancellationToken))
        {
            yield return item;
        }
    }

    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(
        this Task<List<T>> sourceTask,
        int batchSize = 100,
        int delayMilliseconds = 100,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var source = await sourceTask;
        await foreach (var item in source.ToAsyncEnumerable(batchSize, delayMilliseconds, cancellationToken))
        {
            yield return item;
        }
    }
}