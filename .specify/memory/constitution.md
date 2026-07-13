<!--
  Sync Impact Report
  Version change: 0.0.0 → 1.0.0 (initial ratification)
  Added principles:
    - I. Test-Driven Development (TDD)
    - II. Behavior-Driven Development (BDD)
    - III. DTO-Only API Surface
    - IV. FluentValidation for All Input
    - V. Structured Error Responses
    - VI. Simplicity & YAGNI
  Added sections:
    - Technical Constraints
    - Development Workflow
    - Governance
  Templates requiring updates:
    - .specify/templates/plan-template.md ✅ aligned (Constitution Check section present)
    - .specify/templates/spec-template.md ✅ aligned (acceptance scenarios match BDD/TDD principles)
    - .specify/templates/tasks-template.md ✅ aligned (test-first ordering present)
  Follow-up TODOs: none
-->

# Acme Webshop API Constitution

## Core Principles

### I. Test-Driven Development (TDD)

Every feature MUST follow the Red-Green-Refactor cycle:

- Tests MUST be written before implementation code
- Tests MUST fail (Red) before any production code is written
- Implementation MUST be the minimum code needed to pass the test (Green)
- Refactoring MUST keep all tests passing
- Every new endpoint or validator MUST have tests: happy path + 2 edge cases + 1 error case minimum
- The project workflow is **Plan > Implement > Simplify > Verify** — all four phases are mandatory

**Rationale**: This is a teaching project for TDD workflows. Skipping tests undermines the project's core purpose.

### II. Behavior-Driven Development (BDD)

All tests MUST be written in BDD style, expressing behavior from the user's perspective:

- Test scenarios MUST follow the **Given-When-Then** pattern to describe preconditions, actions, and expected outcomes
- Test names MUST be descriptive and read as behavior specifications (e.g., `Create_WithEmptyItems_ShouldReturnValidationError`)
- Acceptance criteria in specifications MUST be written as BDD scenarios before implementation begins
- Tests MUST describe *what* the system does, not *how* it does it — focus on observable behavior, not internal implementation
- Feature specifications MUST include user stories with prioritized BDD acceptance scenarios (Given/When/Then)

**Rationale**: BDD bridges the gap between requirements and tests. Writing scenarios in natural language ensures stakeholders and developers share the same understanding of expected behavior.

### III. DTO-Only API Surface

API endpoints MUST never expose internal entities directly:

- Entities (`ProductEntity`, `OrderEntity`) are internal implementation details
- DTOs (`ProductDto`, `OrderDto`) are the only types returned from or accepted by API endpoints
- Mapping between entities and DTOs MUST live in each controller's private `MapToDto` method
- Request types (`CreateProductRequest`, `CreateOrderRequest`) MUST be separate from entities and DTOs

**Rationale**: Decoupling the API contract from internal storage models prevents breaking changes when the data layer evolves.

### IV. FluentValidation for All Input

All user input MUST be validated via FluentValidation:

- Validators MUST be placed in the `Validators/` directory
- Validators MUST be auto-registered via `AddValidatorsFromAssemblyContaining<Program>()`
- `[Required]` or other data annotations MUST NOT be used as the sole validation mechanism
- Each validator MUST enforce domain-specific rules (e.g., price > 0, non-empty collections, valid enum values)

**Rationale**: Centralizing validation in FluentValidation keeps controllers thin and validation rules testable in isolation.

### V. Structured Error Responses

All API error responses MUST follow a consistent format:

- Response body: `{ error = "human-readable message" }` — no exceptions
- Status codes: 400 for validation errors, 404 for not found, 500 for unexpected failures
- Error messages MUST be descriptive enough for API consumers to understand the problem

**Rationale**: Consistent error shapes simplify client integration and debugging.

### VI. Simplicity & YAGNI

Code MUST be the minimum needed for the current requirement:

- No abstractions for single-use cases — three similar lines are better than a premature helper
- No feature flags, backwards-compatibility shims, or hypothetical future-proofing
- In-memory static stores are sufficient for this demo project — do not introduce EF Core or databases without explicit need
- If code is unused, delete it completely — no `_unused` variables or `// removed` comments

**Rationale**: This is a demo/teaching project. Over-engineering obscures the TDD patterns being taught.

## Technical Constraints

- **Framework**: ASP.NET 8 Minimal Hosting with `[ApiController]` controllers
- **Async**: All I/O-bound operations MUST use `async/await` — never `.Result` or `.Wait()`
- **Logging**: MUST use `ILogger<T>` — never `Console.WriteLine`
- **Testing**: xUnit + FluentAssertions — run with `dotnet test`
- **Build**: `dotnet build --warnaserror` MUST pass with zero warnings
- **Data**: In-memory static stores (`ProductStore`, `OrderStore`) — replace with EF Core only when production use requires it

## Development Workflow

All changes MUST follow this four-phase workflow:

1. **Plan** — Use Plan Mode. Break work into phases. Get approval before coding.
2. **Implement** — TDD + BDD: write one failing test using Given-When-Then, make it pass, repeat.
3. **Simplify** — Remove over-engineering while keeping all tests green.
4. **Verify** — Run `dotnet build --warnaserror && dotnet test`. Fix any failures. Code review before commit.

Git conventions:

- Feature branches: `feature/description`
- Imperative commit messages: "Add endpoint", not "Added endpoint"
- Verify MUST pass before every commit

## Governance

This constitution is the authoritative source of project principles. All code reviews, PRs, and implementation decisions MUST comply with these principles.

- **Amendments**: Any change to this constitution MUST be documented with a version bump, rationale, and sync impact report.
- **Versioning**: Semantic versioning — MAJOR for principle removals/redefinitions, MINOR for additions, PATCH for clarifications.
- **Compliance**: Every PR MUST be checked against these principles before merge. Violations MUST be resolved, not deferred.
- **Runtime guidance**: See `.claude/rules/architecture.md` and `.claude/rules/testing.md` for detailed implementation patterns.

**Version**: 1.0.0 | **Ratified**: 2026-02-26 | **Last Amended**: 2026-02-26
