namespace AsyncEnumerableApi.Models;

public record DataItem
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public decimal Value { get; init; }
}