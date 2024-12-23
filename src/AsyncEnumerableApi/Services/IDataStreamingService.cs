using AsyncEnumerableApi.Models;

namespace AsyncEnumerableApi.Services;

public interface IDataStreamingService
{
    IAsyncEnumerable<DataItem> StreamDataAsync(
        int totalItems,
        int batchSize = 100,
        int delayMilliseconds = 100,
        CancellationToken cancellationToken = default);
}