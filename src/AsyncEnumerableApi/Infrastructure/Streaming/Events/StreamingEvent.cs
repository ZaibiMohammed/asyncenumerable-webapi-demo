namespace AsyncEnumerableApi.Infrastructure.Streaming.Events;

public record StreamStartedEvent(
    string StreamId,
    Type ItemType,
    int BatchSize,
    DateTime StartTime);

public record StreamProgressEvent(
    string StreamId,
    int ItemsProcessed,
    int TotalItems,
    TimeSpan ElapsedTime);

public record StreamCompletedEvent(
    string StreamId,
    int TotalItems,
    TimeSpan TotalTime);

public record StreamErrorEvent(
    string StreamId,
    Exception Error,
    int ItemsProcessedBeforeError);