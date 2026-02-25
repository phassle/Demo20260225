using Microsoft.AspNetCore.Mvc;
using AcmeApi.Models;

namespace AcmeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(ILogger<OrdersController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        _logger.LogInformation("Getting all orders");
        var dtos = OrderStore.Orders.Select(MapToDto).ToList();
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        _logger.LogInformation("Getting order with id: {OrderId}", id);
        var order = OrderStore.Orders.FirstOrDefault(o => o.Id == id);

        if (order == null)
        {
            _logger.LogWarning("Order not found with id: {OrderId}", id);
            return NotFound(new { error = $"Order with id {id} not found" });
        }

        return Ok(MapToDto(order));
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateOrderRequest request)
    {
        _logger.LogInformation("Creating new order for customer: {CustomerEmail}", request.CustomerEmail);

        var entity = new OrderEntity
        {
            Id = Guid.NewGuid(),
            CustomerEmail = request.CustomerEmail,
            Items = request.Items.Select(i => new OrderItem { ProductId = i.ProductId, Quantity = i.Quantity }).ToList(),
            Status = "pending",
            CreatedAt = DateTime.UtcNow
        };

        OrderStore.Orders.Add(entity);
        _logger.LogInformation("Order created with id: {OrderId}", entity.Id);

        var dto = MapToDto(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, dto);
    }

    private static OrderDto MapToDto(OrderEntity entity)
    {
        var itemDtos = entity.Items.Select(i => new OrderItemDto(i.ProductId, i.Quantity)).ToList();
        return new OrderDto(entity.Id, entity.CustomerEmail, itemDtos, entity.Status, entity.CreatedAt);
    }
}
