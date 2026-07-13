# Research: Order Status Workflow

## Decision 1: Status Transition Approach

**Decision**: Use a static `Dictionary<string, HashSet<string>>` mapping current status to allowed next statuses, defined directly in the controller.

**Rationale**: The transition graph is small (5 states, 5 edges) and unlikely to change. A dictionary lookup is clear, testable, and requires no external library.

**Alternatives considered**:
- State machine library (Stateless): Over-engineered for 5 states. Adds a dependency for no benefit.
- Switch/case in controller: Harder to read and maintain than a declarative dictionary.
- Separate service class: Unnecessary abstraction for a single method — violates Simplicity principle.

## Decision 2: HTTP Status Code for Invalid Transitions

**Decision**: Return `422 Unprocessable Entity` for invalid state transitions, distinct from `400 Bad Request` for input validation errors.

**Rationale**: 422 signals "I understand your request format, but it violates business rules." This matches the issue specification and distinguishes validation errors (malformed input) from domain errors (invalid transition).

**Alternatives considered**:
- 400 for everything: Loses the distinction between "bad input" and "valid input, wrong business state."
- 409 Conflict: Typically used for resource conflicts (e.g., duplicate creation), not business rule violations.

## Decision 3: Audit Trail Storage

**Decision**: Add a `List<StatusHistoryEntry>` to `OrderEntity` where `StatusHistoryEntry` contains `Status` and `ChangedAt`. Initialize with the "pending" entry on order creation.

**Rationale**: Keeps audit trail co-located with the order. For an in-memory demo, there's no reason to separate it into its own store.

**Alternatives considered**:
- Separate `StatusHistoryStore`: Unnecessary complexity for a demo project.
- Event sourcing: Massively over-engineered for this scope.

## Decision 4: Status Values — Strings vs Enum

**Decision**: Keep status as `string` to match the existing `OrderEntity.Status` field. Validate allowed values in `UpdateOrderStatusValidator`.

**Rationale**: The existing codebase uses `string` for status. Introducing an enum would require changing `OrderEntity`, `OrderDto`, and the existing `Create` endpoint — out of scope for this feature.

**Alternatives considered**:
- Convert to enum: Would be cleaner long-term but requires changes across the entire order model, breaking the scope of this issue.

## Decision 5: Test Strategy

**Decision**: Use `WebApplicationFactory<Program>` integration tests in a new `OrderStatusWorkflowTests.cs` class, following BDD naming convention.

**Rationale**: Matches the existing `ProductsControllerTests` pattern. Integration tests verify the full pipeline (routing, validation, controller logic, response shape). Separate from existing `OrderValidationTests` which tests the `CreateOrderValidator` in isolation.

**Alternatives considered**:
- Unit tests only: Would miss routing and middleware integration.
- Mixed unit + integration: Unit tests for the transition dictionary add little value since integration tests cover it end-to-end.
