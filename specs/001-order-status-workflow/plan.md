# Implementation Plan: Order Status Workflow

**Branch**: `001-order-status-workflow` | **Date**: 2026-02-26 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-order-status-workflow/spec.md`

## Summary

Add a PATCH `/api/orders/{id}/status` endpoint that transitions orders through a defined lifecycle (pending -> confirmed -> shipped -> delivered, with cancellation from pending/confirmed). Includes FluentValidation for input, transition guard logic, audit trail with status history, and comprehensive BDD-style tests.

## Technical Context

**Language/Version**: C# / .NET 8
**Primary Dependencies**: ASP.NET 8, FluentValidation, xUnit, FluentAssertions, Microsoft.AspNetCore.Mvc.Testing
**Storage**: In-memory static store (`OrderStore`)
**Testing**: xUnit + FluentAssertions + `WebApplicationFactory<Program>`
**Target Platform**: Cross-platform (demo/teaching project)
**Project Type**: Web API
**Performance Goals**: N/A (demo project)
**Constraints**: No database, no EF Core — in-memory only
**Scale/Scope**: Single API, ~8 endpoints after this feature

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Evidence |
|-----------|--------|----------|
| I. TDD | PASS | Tests written before implementation, Red-Green-Refactor cycle |
| II. BDD | PASS | All tests use Given-When-Then naming and scenarios |
| III. DTO-Only | PASS | New `UpdateOrderStatusRequest` for input, `OrderDto` extended for output, entities never exposed |
| IV. FluentValidation | PASS | New `UpdateOrderStatusValidator` in `Validators/`, auto-registered |
| V. Structured Errors | PASS | 400 for validation, 404 for not found, 422 for invalid transition — all return `{ error = "..." }` |
| VI. Simplicity | PASS | Transition map is a simple dictionary, no state machine framework, no service layer abstraction |

## Project Structure

### Documentation (this feature)

```text
specs/001-order-status-workflow/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output
│   └── patch-order-status.md
└── tasks.md             # Phase 2 output (/speckit.tasks)
```

### Source Code (repository root)

```text
acme-api/
  src/AcmeApi/
    Controllers/OrdersController.cs    # Add PATCH action + transition logic
    Models/Order.cs                    # Add StatusHistoryEntry, UpdateOrderStatusRequest, extend OrderDto
    Validators/UpdateOrderStatusValidator.cs  # New validator
  tests/AcmeApi.Tests/
    OrderStatusWorkflowTests.cs        # New test class (~15+ tests)
```

**Structure Decision**: Follows existing single-project layout. All changes fit within existing directories — no new folders needed in `src/`.

## Complexity Tracking

> No constitution violations — this section is empty.
