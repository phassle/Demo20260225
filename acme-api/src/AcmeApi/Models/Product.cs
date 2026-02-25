namespace AcmeApi.Models;

/// <summary>
/// Product entity - internal representation, never returned from API
/// </summary>
public class ProductEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool InStock { get; set; }
}

/// <summary>
/// Product DTO - returned from API endpoints
/// </summary>
public record ProductDto(Guid Id, string Name, decimal Price, string Category, bool InStock);

/// <summary>
/// Request model for creating a product
/// </summary>
public record CreateProductRequest(string Name, decimal Price, string Category, bool InStock);

/// <summary>
/// In-memory product store for demo purposes
/// </summary>
public static class ProductStore
{
    public static List<ProductEntity> Products = new()
    {
        new()
        {
            Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
            Name = "Wireless Keyboard",
            Price = 79.99m,
            Category = "electronics",
            InStock = true
        },
        new()
        {
            Id = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
            Name = "C# in Depth",
            Price = 39.99m,
            Category = "books",
            InStock = true
        },
        new()
        {
            Id = Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012"),
            Name = "Ergonomic Mouse",
            Price = 59.99m,
            Category = "electronics",
            InStock = false
        }
    };
}
