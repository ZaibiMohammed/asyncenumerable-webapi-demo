namespace AsyncEnumerableApi.Models;

public record Product
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public DateTime LastUpdated { get; init; }
    public string Supplier { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public string Condition { get; init; } = string.Empty;
    public string Manufacturer { get; init; } = string.Empty;
    public double Rating { get; init; }
    public int ReviewCount { get; init; }
    public string SKU { get; init; } = string.Empty;
    public double Weight { get; init; }
    public string Dimensions { get; init; } = string.Empty;
    public bool IsAvailable { get; init; }
}