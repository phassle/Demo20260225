# Acme Webshop API

## About
C# ASP.NET 8 Web API. FluentValidation for input. xUnit + FluentAssertions for testing. In-memory data store (demo).

## Commands
- Run API: `dotnet run --project src/AcmeApi`
- Run tests: `dotnet test`
- Build: `dotnet build --warnaserror`
- Verify all: `dotnet build --warnaserror && dotnet test`

## Workflow — Plan → Implement → Simplify → Verify

Follow this cycle for every feature or bug fix:

1. **Plan** — Use Plan Mode (Shift+Tab). Break into phases. Get approval before writing code.
2. **Implement** — TDD: one failing test → make it pass → repeat. Never write multiple tests at once.
3. **Simplify** — Remove over-engineering, improve names, reduce duplication. All tests must stay green.
4. **Verify** — Run `dotnet build --warnaserror && dotnet test`. Fix anything that fails. Do not skip this step.

## Rules
- Be extremely concise. Sacrifice grammar for concision.
- At the end of each plan, list unresolved questions (if any).
- Always use DTOs — NEVER expose EF entities from API
- FluentValidation for all input — never [Required] alone
- async/await — never .Result or .Wait()
- ILogger<T> — never Console.WriteLine
- Every new endpoint MUST have xUnit tests
- Use `{ error = "..." }` pattern for error responses
- Run `dotnet build --warnaserror && dotnet test` before every commit

## Architecture
```
src/AcmeApi/
├── Controllers/       ← One per resource (ProductsController, OrdersController)
├── Models/            ← Entities + DTOs + in-memory stores
├── Validators/        ← FluentValidation validators
└── Program.cs         ← Entry point, DI config
tests/AcmeApi.Tests/   ← xUnit + FluentAssertions
```

## Git workflow
- Feature branches: `feature/description`
- Commit messages: imperative mood ("Add endpoint", not "Added endpoint")
- Always run `dotnet build --warnaserror && dotnet test` before committing

## Additional docs
- See `.claude/rules/architecture.md` for controller patterns, validation patterns, error conventions
- See `.claude/rules/testing.md` for xUnit framework, naming, coverage expectations
