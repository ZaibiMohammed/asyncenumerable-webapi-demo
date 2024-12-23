namespace AsyncEnumerableApi.Infrastructure.AsyncEnumerable;

public static class AsyncEnumerableExtensions
{
    public static async IAsyncEnumerable<T> WithDelay<T>(
        this IAsyncEnumerable<T> source,
        int delayMilliseconds,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var item in source.WithCancellation(cancellationToken))
        {
            yield return item;
            if (delayMilliseconds > 0)
            {
                await Task.Delay(delayMilliseconds, cancellationToken);
            }
        }
    }

    public static async IAsyncEnumerable<T> WithFilter<T>(
        this IAsyncEnumerable<T> source,
        Func<T, bool> predicate,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var item in source.WithCancellation(cancellationToken))
        {
            if (predicate(item))
            {
                yield return item;
            }
        }
    }

    public static async IAsyncEnumerable<T> WithBatchDelay<T>(
        this IAsyncEnumerable<T> source,
        int batchSize,
        int delayMilliseconds,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int count = 0;
        await foreach (var item in source.WithCancellation(cancellationToken))
        {
            yield return item;
            count++;

            if (count % batchSize == 0 && delayMilliseconds > 0)
            {
                await Task.Delay(delayMilliseconds, cancellationToken);
            }
        }
    }

    public static async IAsyncEnumerable<T> WithProgress<T>(
        this IAsyncEnumerable<T> source,
        Action<int> progressAction,
        int reportEvery = 100,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int count = 0;
        await foreach (var item in source.WithCancellation(cancellationToken))
        {
            yield return item;
            count++;

            if (count % reportEvery == 0)
            {
                progressAction(count);
            }
        }
    }
}