using System.Threading.Channels;
using AsyncEnumerableApi.Infrastructure.EventBus;
using AsyncEnumerableApi.Infrastructure.Streaming.Events;

namespace AsyncEnumerableApi.Infrastructure.Streaming.Channels;

public class StreamingChannel<T>
{
    private readonly Channel<T> _channel;
    private readonly IEventBus _eventBus;
    private readonly string _streamId;
    private readonly StreamingOptions _options;
    private int _itemsProcessed;
    private readonly Stopwatch _stopwatch;

    public StreamingChannel(
        IEventBus eventBus,
        StreamingOptions options)
    {
        _eventBus = eventBus;
        _options = options;
        _streamId = Guid.NewGuid().ToString();
        _stopwatch = new Stopwatch();

        var channelOptions = new BoundedChannelOptions(_options.MaxBufferSize)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = true
        };

        _channel = Channel.CreateBounded<T>(channelOptions);
    }

    public async Task WriteAsync(
        IAsyncEnumerable<T> source,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _stopwatch.Start();
            await _eventBus.PublishAsync(new StreamStartedEvent(
                _streamId,
                typeof(T),
                _options.BatchSize,
                DateTime.UtcNow));

            await foreach (var item in source.WithCancellation(cancellationToken))
            {
                await _channel.Writer.WriteAsync(item, cancellationToken);
                _itemsProcessed++;

                if (_itemsProcessed % _options.ProgressUpdateInterval == 0)
                {
                    await _eventBus.PublishAsync(new StreamProgressEvent(
                        _streamId,
                        _itemsProcessed,
                        _options.ExpectedItemCount ?? -1,
                        _stopwatch.Elapsed));
                }

                if (_options.DelayBetweenItems > 0)
                {
                    await Task.Delay(_options.DelayBetweenItems, cancellationToken);
                }
            }

            _channel.Writer.Complete();
            _stopwatch.Stop();

            await _eventBus.PublishAsync(new StreamCompletedEvent(
                _streamId,
                _itemsProcessed,
                _stopwatch.Elapsed));
        }
        catch (Exception ex)
        {
            _channel.Writer.Complete(ex);
            await _eventBus.PublishAsync(new StreamErrorEvent(
                _streamId,
                ex,
                _itemsProcessed));
            throw;
        }
    }

    public IAsyncEnumerable<T> ReadAsync(CancellationToken cancellationToken = default)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}