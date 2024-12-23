namespace AsyncEnumerableApi.Models;

public record Product
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public DateTime LastUpdated { get; init; }
}

public static class SeedData
{
    private static readonly string[] Categories = {
        "Electronics", "Books", "Clothing", "Home & Garden", "Sports"
    };

    private static readonly Dictionary<string, (string[] Products, decimal MinPrice, decimal MaxPrice)> CategoryProducts = 
        new()
        {
            ["Electronics"] = (new[] {
                "Smartphone", "Laptop", "Tablet", "Smartwatch", "Headphones",
                "Camera", "Speaker", "Monitor", "Keyboard", "Mouse"
            }, 99.99m, 2499.99m),
            
            ["Books"] = (new[] {
                "Novel", "Textbook", "Biography", "Cookbook", "Science Fiction",
                "Mystery", "History Book", "Self-Help", "Comic Book", "Children's Book"
            }, 9.99m, 99.99m),
            
            ["Clothing"] = (new[] {
                "T-Shirt", "Jeans", "Dress", "Jacket", "Sweater",
                "Shorts", "Skirt", "Coat", "Pants", "Shirt"
            }, 19.99m, 199.99m),
            
            ["Home & Garden"] = (new[] {
                "Plant Pot", "Garden Tools", "Lamp", "Cushion", "Rug",
                "Curtains", "Bedding Set", "Storage Box", "Mirror", "Clock"
            }, 14.99m, 299.99m),
            
            ["Sports"] = (new[] {
                "Running Shoes", "Yoga Mat", "Dumbbells", "Tennis Racket", "Basketball",
                "Football", "Bicycle", "Swimming Goggles", "Boxing Gloves", "Skateboard"
            }, 24.99m, 899.99m)
        };

    public static IEnumerable<Product> GenerateProducts(int count)
    {
        var random = new Random(42); // Fixed seed for consistent data
        var products = new List<Product>();
        var baseDate = DateTime.Now.AddMonths(-1);

        for (int i = 1; i <= count; i++)
        {
            var category = Categories[random.Next(Categories.Length)];
            var (categoryProducts, minPrice, maxPrice) = CategoryProducts[category];
            var productName = categoryProducts[random.Next(categoryProducts.Length)];
            
            var priceRange = maxPrice - minPrice;
            var price = minPrice + (decimal)(random.NextDouble() * (double)priceRange);
            price = decimal.Round(price, 2);

            products.Add(new Product
            {
                Id = i,
                Name = $"{productName} {random.Next(1000)}",
                Category = category,
                Price = price,
                Stock = random.Next(0, 1000),
                LastUpdated = baseDate.AddMinutes(random.Next(0, 44640)) // Random time within last month
            });
        }

        return products;
    }
}