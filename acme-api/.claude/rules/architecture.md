# Architectural Patterns — Acme API

## Controller pattern
Every controller inherits `ControllerBase` and uses `[ApiController]`:
```csharp
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
    public ActionResult<List<ProductDto>> GetAll() { ... }

    [HttpGet("{id:guid}")]
    public ActionResult<ProductDto> GetById(Guid id) { ... }

    [HttpPost]
    public ActionResult<ProductDto> Create(CreateProductRequest request) { ... }
}
```

## Validation pattern
All input goes through FluentValidation validators in `Validators/`:
```csharp
public class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
```
Validators are auto-registered via `AddValidatorsFromAssemblyContaining<Program>()`.
Inject `IValidator<T>` in controllers. Never use [Required] alone.

## Error response pattern
Always return `{ error = "human-readable message" }` with appropriate status code:
- 400 = validation error
- 404 = resource not found
- 500 = unexpected server error

## DTO pattern
- Entities (`ProductEntity`) are internal — never returned from API
- DTOs (`ProductDto`, `CreateProductRequest`) are what the API returns/accepts
- Map between them in the controller

## Data layer
Currently in-memory stores (demo project). In production: replace with EF Core.
Each model file exports: entity class, DTOs, and a static store class.
