using Microsoft.AspNetCore.Mvc;
using AcmeApi.Models;

namespace AcmeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ILogger<ProductsController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        _logger.LogInformation("Getting all products");
        var dtos = ProductStore.Products.Select(MapToDto).ToList();
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        _logger.LogInformation("Getting product with id: {ProductId}", id);
        var product = ProductStore.Products.FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            _logger.LogWarning("Product not found with id: {ProductId}", id);
            return NotFound(new { error = $"Product with id {id} not found" });
        }

        return Ok(MapToDto(product));
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateProductRequest request)
    {
        _logger.LogInformation("Creating new product: {ProductName}", request.Name);

        var entity = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price,
            Category = request.Category,
            InStock = request.InStock
        };

        ProductStore.Products.Add(entity);
        _logger.LogInformation("Product created with id: {ProductId}", entity.Id);

        var dto = MapToDto(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, dto);
    }

    private static ProductDto MapToDto(ProductEntity entity)
    {
        return new ProductDto(entity.Id, entity.Name, entity.Price, entity.Category, entity.InStock);
    }
}
