using AsyncEnumerableApi.Models;

namespace AsyncEnumerableApi.Services;

public class LargeDataGenerator
{
    private static readonly string[] Cities = {
        "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia",
        "San Antonio", "San Diego", "Dallas", "San Jose", "Austin", "Jacksonville",
        "Fort Worth", "San Francisco", "Columbus", "Charlotte", "Detroit", "Seattle"
    };

    private static readonly string[] Suppliers = {
        "TechCorp", "GlobalSupply", "MegaVendor", "PrimeSellers", "TopDistributors",
        "QualityGoods", "BestValue", "SuperStore", "WholesaleKing", "SmartSupply"
    };

    private static readonly Dictionary<string, (string[] Names, decimal MinPrice, decimal MaxPrice)> ProductsByCategory = 
        new()
        {
            ["Electronics"] = (new[] {
                "Laptop", "Smartphone", "Tablet", "Smartwatch", "Headphones",
                "Camera", "Speaker", "Monitor", "Keyboard", "Mouse", "TV",
                "Gaming Console", "Drone", "VR Headset", "Smart Home Hub"
            }, 99.99m, 2499.99m),
            
            ["Books"] = (new[] {
                "Mystery Novel", "Science Fiction", "Biography", "Cookbook", "History Book",
                "Self-Help Guide", "Technical Manual", "Children's Book", "Business Book",
                "Travel Guide", "Art Book", "Poetry Collection", "Educational Textbook"
            }, 9.99m, 199.99m),
            
            ["Clothing"] = (new[] {
                "T-Shirt", "Jeans", "Dress", "Jacket", "Sweater", "Coat",
                "Shorts", "Skirt", "Pants", "Shirt", "Hoodie", "Socks",
                "Underwear", "Swimwear", "Sportswear"
            }, 19.99m, 299.99m),
            
            ["Home & Garden"] = (new[] {
                "Plant Pot", "Garden Tools", "Lamp", "Cushion", "Rug",
                "Curtains", "Bedding Set", "Storage Box", "Mirror", "Clock",
                "Furniture", "Kitchen Appliance", "Decoration", "Cleaning Tools"
            }, 14.99m, 999.99m),
            
            ["Sports"] = (new[] {
                "Running Shoes", "Yoga Mat", "Dumbbells", "Tennis Racket", "Basketball",
                "Football", "Bicycle", "Swimming Goggles", "Boxing Gloves", "Skateboard",
                "Golf Clubs", "Fitness Tracker", "Exercise Bike", "Weight Bench"
            }, 24.99m, 1499.99m),

            ["Beauty & Personal Care"] = (new[] {
                "Shampoo", "Conditioner", "Face Cream", "Body Lotion", "Perfume",
                "Makeup Kit", "Hair Dryer", "Electric Shaver", "Nail Polish", "Sunscreen",
                "Face Mask", "Hair Styling Tools", "Skin Care Set", "Body Wash"
            }, 9.99m, 299.99m),

            ["Automotive"] = (new[] {
                "Car Parts", "Motor Oil", "Car Accessories", "Tools Set", "Car Care Kit",
                "Battery", "Tires", "Car Electronics", "Interior Accessories", "Exterior Accessories",
                "Performance Parts", "Safety Equipment", "Cleaning Supplies"
            }, 19.99m, 999.99m)
        };

    private static readonly string[] Conditions = { "New", "Refurbished", "Open Box", "Used - Like New", "Used - Good" };
    private static readonly string[] Manufacturers = { "Samsung", "Apple", "Sony", "LG", "Dell", "HP", "Lenovo", "Asus", "Acer", "Microsoft" };

    public static IEnumerable<Product> GenerateLargeDataset(int count)
    {
        var random = new Random(42); // Fixed seed for consistent data
        var baseDate = DateTime.Now.AddMonths(-1);

        for (int i = 1; i <= count; i++)
        {
            var category = ProductsByCategory.Keys.ElementAt(random.Next(ProductsByCategory.Count));
            var (products, minPrice, maxPrice) = ProductsByCategory[category];
            var productBase = products[random.Next(products.Length)];
            
            var priceRange = maxPrice - minPrice;
            var price = minPrice + (decimal)(random.NextDouble() * (double)priceRange);
            price = decimal.Round(price, 2);

            var supplier = Suppliers[random.Next(Suppliers.Length)];
            var city = Cities[random.Next(Cities.Length)];
            var condition = Conditions[random.Next(Conditions.Length)];
            var manufacturer = Manufacturers[random.Next(Manufacturers.Length)];

            var product = new Product
            {
                Id = i,
                Name = $"{manufacturer} {productBase} {random.Next(1000)}",
                Category = category,
                Price = price,
                Stock = random.Next(0, 1000),
                LastUpdated = baseDate.AddMinutes(random.Next(0, 44640)), // Random time within last month
                Supplier = supplier,
                Location = city,
                Condition = condition,
                Manufacturer = manufacturer,
                Rating = Math.Round(3.0 + random.NextDouble() * 2.0, 1), // Rating between 3.0 and 5.0
                ReviewCount = random.Next(0, 1000),
                SKU = $"{category.Substring(0, 2).ToUpper()}{i:D6}",
                Weight = Math.Round(0.1 + random.NextDouble() * 19.9, 2), // Weight between 0.1 and 20.0 kg
                Dimensions = $"{random.Next(1, 100)}x{random.Next(1, 100)}x{random.Next(1, 100)}",
                IsAvailable = random.NextDouble() > 0.1 // 90% chance of being available
            };

            yield return product;
        }
    }
}