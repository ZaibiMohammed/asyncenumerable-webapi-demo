using AsyncEnumerableApi.Models;

namespace AsyncEnumerableApi.Services;

public class DataStreamingService : IDataStreamingService
{
    private readonly Random _random = new();

    public async IAsyncEnumerable<DataItem> StreamDataAsync(
        int totalItems,
        int batchSize = 100,
        int delayMilliseconds = 100,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var processedItems = 0;
        
        while (processedItems < totalItems)
        {
            // Check for cancellation
            cancellationToken.ThrowIfCancellationRequested();

            var currentBatchSize = Math.Min(batchSize, totalItems - processedItems);
            var batch = GenerateBatch(processedItems, currentBatchSize);

            foreach (var item in batch)
            {
                yield return item;
            }

            processedItems += currentBatchSize;

            // Simulate processing delay
            if (delayMilliseconds > 0)
            {
                await Task.Delay(delayMilliseconds, cancellationToken);
            }
        }
    }

    private IEnumerable<DataItem> GenerateBatch(int startId, int count)
    {
        return Enumerable.Range(startId, count)
            .Select(id => new DataItem
            {
                Id = id,
                Name = $"Item {id}",
                Timestamp = DateTime.UtcNow,
                Value = (decimal)(_random.NextDouble() * 1000)
            });
    }
}