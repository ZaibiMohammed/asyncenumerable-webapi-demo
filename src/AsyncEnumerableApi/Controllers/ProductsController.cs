using AsyncEnumerableApi.Models;
using AsyncEnumerableApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AsyncEnumerableApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductService productService,
        ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// Streams products with optional filtering
    /// </summary>
    [HttpGet("stream")]
    public IAsyncEnumerable<Product> GetProductsStream(
        [FromQuery] int? pageSize = null,
        [FromQuery] string? category = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Streaming products with filters - Category: {Category}, MinPrice: {MinPrice}, MaxPrice: {MaxPrice}",
            category, minPrice, maxPrice);

        return _productService.GetProductsStreamAsync(
            pageSize, category, minPrice, maxPrice, cancellationToken);
    }

    /// <summary>
    /// Gets product categories
    /// </summary>
    [HttpGet("categories")]
    public ActionResult<IEnumerable<string>> GetCategories()
    {
        return Ok(new[] { "Electronics", "Books", "Clothing", "Home & Garden", "Sports" });
    }
}