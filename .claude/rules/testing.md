# Testing Conventions — Acme API

## Framework
xUnit + FluentAssertions. Run with `dotnet test`.

## Structure
Tests mirror `src/` structure:
```
src/AcmeApi/Models/Product.cs         → tests/ProductValidationTests.cs
src/AcmeApi/Models/Order.cs           → tests/OrderValidationTests.cs
src/AcmeApi/Controllers/Products...   → tests/ProductsControllerTests.cs (if needed)
```

## What to test
- **Validators**: happy path + edge cases (empty strings, negative numbers, invalid enums)
- **Controllers**: response status codes, response body shape, error cases (via WebApplicationFactory)
- **Services**: pure functions, business logic

## Naming
Use descriptive names: `RejectsNegativePrice`, not `TestCase3`.
Class: `ProductValidationTests`. Method: `Create_WithNegativePrice_ShouldFail`.

## Coverage expectations
Every new endpoint or validator must have tests before the PR is opened.
Minimum: happy path + 2 edge cases + 1 error case.
