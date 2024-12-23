using AsyncEnumerableApi.Models;

namespace AsyncEnumerableApi.Services;

public interface IProductService
{
    IAsyncEnumerable<Product> GetProductsStreamAsync(
        int? pageSize = null,
        string? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        CancellationToken cancellationToken = default);
}

public class ProductService : IProductService
{
    private readonly ILogger<ProductService> _logger;
    private readonly List<Product> _products;

    public ProductService(ILogger<ProductService> logger)
    {
        _logger = logger;
        // Generate 1000 sample products
        _products = SeedData.GenerateProducts(1000).ToList();
    }

    public async IAsyncEnumerable<Product> GetProductsStreamAsync(
        int? pageSize = null,
        string? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var query = _products.AsQueryable();

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(p => p.Category == category);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        var batchSize = pageSize ?? 50;
        var results = query.AsEnumerable();

        foreach (var batch in results.Chunk(batchSize))
        {
            foreach (var product in batch)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return product;
            }

            await Task.Delay(100, cancellationToken); // Simulate network delay
        }
    }
}