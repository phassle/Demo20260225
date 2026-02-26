# Acme Webshop API

## What

ASP.NET 8 Web API — demo webshop with products and orders.

| Layer        | Technology                          |
|--------------|-------------------------------------|
| Framework    | ASP.NET 8 Minimal Hosting           |
| Validation   | FluentValidation (auto-registered)  |
| Testing      | xUnit + FluentAssertions            |
| Data         | In-memory static stores (demo only) |

## Why

Teaching / demo project for TDD workflows with Claude Code. Ships with a known bug (empty order items accepted) to demonstrate the fix cycle.

## Commands

```
dotnet run --project acme-api/src/AcmeApi     # Run API (Swagger at /swagger)
dotnet test -p acme-api                        # Run tests
dotnet build --warnaserror                     # Build (warnings = errors)
dotnet build --warnaserror && dotnet test       # Verify all (run before every commit)
```

## Project layout

```
acme-api/
  src/AcmeApi/
    Program.cs              Entry point, DI, middleware
    Controllers/            One controller per resource
    Models/                 Entities + DTOs + static stores
    Validators/             FluentValidation validators
  tests/AcmeApi.Tests/      xUnit test classes
```

## Endpoints

| Method | Route              | Controller                          | Purpose         |
|--------|--------------------|-------------------------------------|-----------------|
| GET    | /api/health        | HealthController.cs:9               | Health check    |
| GET    | /api/products      | ProductsController.cs:18            | List products   |
| GET    | /api/products/{id} | ProductsController.cs:25            | Get by ID       |
| POST   | /api/products      | ProductsController.cs:40            | Create product  |
| DELETE | /api/products/{id} | ProductsController.cs:61            | Delete product  |
| GET    | /api/orders        | OrdersController.cs:18              | List orders     |
| GET    | /api/orders/{id}   | OrdersController.cs:25              | Get by ID       |
| POST   | /api/orders        | OrdersController.cs:40              | Create order    |

All paths relative to `acme-api/src/AcmeApi/Controllers/`.

## Key patterns

**Entity/DTO separation** — Entities are internal (`ProductEntity`, `OrderEntity`). API returns records (`ProductDto`, `OrderDto`). Mapping lives in each controller's private `MapToDto` method.
- Product model: `Models/Product.cs:6` (entity), `:18` (DTO), `:23` (request)
- Order model: `Models/Order.cs:15` (entity), `:32` (DTO), `:37` (request)

**Validation** — All input validated via FluentValidation. Validators auto-registered at `Program.cs:9`. Never use `[Required]` alone.
- Product rules: `Validators/CreateProductValidator.cs:8` — name, price > 0, category enum
- Order rules: `Validators/CreateOrderValidator.cs:8` — email, items not null, item fields

**Error responses** — Always `{ error = "message" }` with correct status code (400/404/500).

**Data stores** — Static in-memory lists. `ProductStore` at `Models/Product.cs:28`, `OrderStore` at `Models/Order.cs:42`. Replace with EF Core for production.

## Known bug (intentional)

`CreateOrderValidator` only checks `NotNull()` on Items — empty lists pass validation.
- Bug location: `Validators/CreateOrderValidator.cs:19`
- Test proving the bug: `tests/AcmeApi.Tests/OrderValidationTests.cs:159`
- Fix: add `.NotEmpty()` or `.Must(x => x.Count > 0)` after `NotNull()`

## Workflow — Plan > Implement > Simplify > Verify

1. **Plan** — Use Plan Mode. Break into phases. Get approval.
2. **Implement** — TDD: one failing test > pass > repeat.
3. **Simplify** — Remove over-engineering. Tests stay green.
4. **Verify** — `dotnet build --warnaserror && dotnet test`. Fix failures.

## Rules

- DTOs only — never expose entities from API
- FluentValidation for all input
- async/await — never `.Result` or `.Wait()`
- `ILogger<T>` — never `Console.WriteLine`
- Every new endpoint must have tests
- `{ error = "..." }` for all error responses
- Verify before every commit

## Git

- Feature branches: `feature/description`
- Imperative commit messages ("Add endpoint", not "Added endpoint")

## Deep dive docs

- `.claude/rules/architecture.md` — controller, validation, DTO, error patterns
- `.claude/rules/testing.md` — framework, naming, structure, coverage expectations
