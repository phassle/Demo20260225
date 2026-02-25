namespace AcmeApi.Models;

/// <summary>
/// Represents a single line item in an order
/// </summary>
public class OrderItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

/// <summary>
/// Order entity - internal representation, never returned from API
/// </summary>
public class OrderEntity
{
    public Guid Id { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public List<OrderItem> Items { get; set; } = new();
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Order item DTO
/// </summary>
public record OrderItemDto(Guid ProductId, int Quantity);

/// <summary>
/// Order DTO - returned from API endpoints
/// </summary>
public record OrderDto(Guid Id, string CustomerEmail, List<OrderItemDto> Items, string Status, DateTime CreatedAt);

/// <summary>
/// Request model for creating an order
/// </summary>
public record CreateOrderRequest(string CustomerEmail, List<OrderItemDto> Items);

/// <summary>
/// In-memory order store for demo purposes
/// </summary>
public static class OrderStore
{
    public static List<OrderEntity> Orders = new()
    {
        new()
        {
            Id = Guid.Parse("d4e5f6a7-b8c9-0123-defa-234567890123"),
            CustomerEmail = "alice@example.com",
            Items = new()
            {
                new()
                {
                    ProductId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                    Quantity = 1
                }
            },
            Status = "shipped",
            CreatedAt = new DateTime(2026, 2, 20, 10, 0, 0, DateTimeKind.Utc)
        }
    };
}
