# Tasks: Order Status Workflow

**Input**: Design documents from `/specs/001-order-status-workflow/`
**Prerequisites**: plan.md (required), spec.md (required), research.md, data-model.md, contracts/

**Tests**: TDD and BDD are mandated by the constitution. Tests MUST be written before implementation using Given-When-Then naming.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

---

## Phase 1: Setup

**Purpose**: No new project initialization needed — existing project structure is used.

- [ ] T001 Verify existing project builds cleanly with `dotnet build --warnaserror` in `acme-api/`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Add shared models, DTOs, and types that ALL user stories depend on

**CRITICAL**: No user story work can begin until this phase is complete

- [ ] T002 Add `StatusHistoryEntry` class to `acme-api/src/AcmeApi/Models/Order.cs` with `Status` (string) and `ChangedAt` (DateTime) properties
- [ ] T003 Add `StatusHistoryEntryDto` record to `acme-api/src/AcmeApi/Models/Order.cs`
- [ ] T004 Add `StatusHistory` property (`List<StatusHistoryEntry>`) to `OrderEntity` in `acme-api/src/AcmeApi/Models/Order.cs`
- [ ] T005 Update `OrderDto` record to include `StatusHistory` (`List<StatusHistoryEntryDto>`) in `acme-api/src/AcmeApi/Models/Order.cs`
- [ ] T006 Update `MapToDto` in `acme-api/src/AcmeApi/Controllers/OrdersController.cs` to map `StatusHistory` from entity to DTO
- [ ] T007 Add `UpdateOrderStatusRequest` record to `acme-api/src/AcmeApi/Models/Order.cs` with `Status` (string) property
- [ ] T008 Update `OrdersController.Create` in `acme-api/src/AcmeApi/Controllers/OrdersController.cs` to initialize `StatusHistory` with initial "pending" entry on order creation
- [ ] T009 Update seed data in `OrderStore` in `acme-api/src/AcmeApi/Models/Order.cs` to include `StatusHistory` for existing seed order
- [ ] T010 Add allowed transitions `Dictionary<string, HashSet<string>>` as a static readonly field in `acme-api/src/AcmeApi/Controllers/OrdersController.cs`
- [ ] T011 Verify build passes with `dotnet build --warnaserror` and existing tests still pass with `dotnet test` in `acme-api/`

**Checkpoint**: Foundation ready — models, DTOs, and transition map in place. User story implementation can begin.

---

## Phase 3: User Story 1 — Update Order Status (Priority: P1)

**Goal**: PATCH endpoint accepts valid transitions and returns updated order with 200

**Independent Test**: Create an order, transition through pending->confirmed->shipped->delivered, verify each returns 200 with updated status

### Tests for User Story 1

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T012 [US1] Create test class `OrderStatusWorkflowTests` with `WebApplicationFactory<Program>` fixture and helper to create a test order via POST `/api/orders` in `acme-api/tests/AcmeApi.Tests/OrderStatusWorkflowTests.cs`
- [ ] T013 [US1] Add test `UpdateStatus_PendingToConfirmed_Returns200WithUpdatedStatus` — Given order with status "pending", When PATCH with "confirmed", Then 200 + status is "confirmed" in `acme-api/tests/AcmeApi.Tests/OrderStatusWorkflowTests.cs`
- [ ] T014 [US1] Add test `UpdateStatus_ConfirmedToShipped_Returns200WithUpdatedStatus` — Given order with status "confirmed", When PATCH with "shipped", Then 200 + status is "shipped" in `acme-api/tests/AcmeApi.Tests/OrderStatusWorkflowTests.cs`
- [ ] T015 [US1] Add test `UpdateStatus_ShippedToDelivered_Returns200WithUpdatedStatus` — Given order with status "shipped", When PATCH with "delivered", Then 200 + status is "delivered" in `acme-api/tests/AcmeApi.Tests/OrderStatusWorkflowTests.cs`
- [ ] T016 [US1] Add test `UpdateStatus_PendingToCancelled_Returns200WithUpdatedStatus` — Given order with status "pending", When PATCH with "cancelled", Then 200 + status is "cancelled" in `acme-api/tests/AcmeApi.Tests/OrderStatusWorkflowTests.cs`
- [ ] T017 [US1] Add test `UpdateStatus_ConfirmedToCancelled_Returns200WithUpdatedStatus` — Given order with status "confirmed", When PATCH with "cancelled", Then 200 + status is "cancelled" in `acme-api/tests/AcmeApi.Tests/OrderStatusWorkflowTests.cs`

### Implementation for User Story 1

- [ ] T018 [US1] Add `UpdateStatus` PATCH action in `acme-api/src/AcmeApi/Controllers/OrdersController.cs` at route `{id}/status` — find order, check transition map, update status, return `Ok(MapToDto(order))`
- [ ] T019 [US1] Run tests and verify all US1 tests pass with `dotnet test` in `acme-api/`

**Checkpoint**: All 5 valid transitions work. Endpoint exists and returns updated order.

---

## Phase 4: User Story 2 — Reject Invalid Transitions (Priority: P1)

**Goal**: Invalid transitions return 422 with descriptive error message

**Independent Test**: Attempt all invalid transitions (shipped->pending, delivered->anything, cancelled->anything, self-transitions) and verify 422 responses

### Tests for User Story 2

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T020 [US2] Add test `UpdateStatus_ShippedToPending_Returns422WithError` — Given order with status "shipped", When PATCH with "pending", Then 422 + error "Cannot transition from 'shipped' to 'pending'" in `acme-api/tests/AcmeApi.Tests/OrderStatusWorkflowTests.cs`
- [ ] T021 [US2] Add test `UpdateStatus_DeliveredToConfirmed_Returns422` — Given order with status "delivered", When PATCH with "confirmed", Then 422 in `acme-api/tests/AcmeApi.Tests/OrderStatusWorkflowTests.cs`
- [ ] T022 [US2] Add test `UpdateStatus_CancelledToPending_Returns422` — Given order with status "cancelled", When PATCH with "pending", Then 422 in `acme-api/tests/AcmeApi.Tests/OrderStatusWorkflowTests.cs`
- [ ] T023 [US2] Add test `UpdateStatus_PendingToPending_Returns422` — Given order with status "pending", When PATCH with "pending" (self-transition), Then 422 in `acme-api/tests/AcmeApi.Tests/OrderStatusWorkflowTests.cs`
- [ ] T024 [US2] Add test `UpdateStatus_DeliveredIsTerminal_Returns422ForAllTransitions` — Given order with status "delivered", When PATCH with any other status, Then 422 in `acme-api/tests/AcmeApi.Tests/OrderStatusWorkflowTests.cs`
- [ ] T025 [US2] Add test `UpdateStatus_CancelledIsTerminal_Returns422ForAllTransitions` — Given order with status "cancelled", When PATCH with any other status, Then 422 in `acme-api/tests/AcmeApi.Tests/OrderStatusWorkflowTests.cs`

### Implementation for User Story 2

- [ ] T026 [US2] Add transition validation to `UpdateStatus` action in `acme-api/src/AcmeApi/Controllers/OrdersController.cs` — if transition not in allowed map, return `UnprocessableEntity(new { error = $"Cannot transition from '{current}' to '{requested}'" })`
- [ ] T027 [US2] Run tests and verify all US1 + US2 tests pass with `dotnet test` in `acme-api/`

**Checkpoint**: All invalid transitions rejected with 422. Valid transitions still work.

---

## Phase 5: User Story 3 — Validate Status Input (Priority: P1)

**Goal**: FluentValidation catches empty/invalid status values before reaching controller logic. 404 for unknown orders.

**Independent Test**: Send empty status, unknown status string, and non-existent order ID — verify 400 and 404 responses

### Tests for User Story 3

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T028 [US3] Add test `UpdateStatus_EmptyStatus_Returns400` — Given an order, When PATCH with empty status, Then 400 in `acme-api/tests/AcmeApi.Tests/OrderStatusWorkflowTests.cs`
- [ ] T029 [US3] Add test `UpdateStatus_InvalidStatusValue_Returns400` — Given an order, When PATCH with status "unknown", Then 400 in `acme-api/tests/AcmeApi.Tests/OrderStatusWorkflowTests.cs`
- [ ] T030 [US3] Add test `UpdateStatus_NonExistentOrder_Returns404` — Given a random Guid, When PATCH with valid status, Then 404 with error message in `acme-api/tests/AcmeApi.Tests/OrderStatusWorkflowTests.cs`

### Implementation for User Story 3

- [ ] T031 [US3] Create `UpdateOrderStatusValidator` in `acme-api/src/AcmeApi/Validators/UpdateOrderStatusValidator.cs` — validate Status NotEmpty and must be one of: pending, confirmed, shipped, delivered, cancelled
- [ ] T032 [US3] Add 404 handling to `UpdateStatus` action in `acme-api/src/AcmeApi/Controllers/OrdersController.cs` — return `NotFound(new { error = $"Order with id {id} not found" })` if order not found
- [ ] T033 [US3] Inject `IValidator<UpdateOrderStatusRequest>` in `OrdersController` and call `ValidateAsync` in the `UpdateStatus` action, returning 400 with `{ error = "..." }` on failure in `acme-api/src/AcmeApi/Controllers/OrdersController.cs`
- [ ] T034 [US3] Run tests and verify all US1 + US2 + US3 tests pass with `dotnet test` in `acme-api/`

**Checkpoint**: Full input validation pipeline works. 400 for bad input, 404 for missing order, 422 for invalid transition, 200 for success.

---

## Phase 6: User Story 4 — Audit Trail (Priority: P2)

**Goal**: Status history recorded on every transition and returned in the order DTO

**Independent Test**: Create order, transition through multiple states, GET the order, verify statusHistory contains all entries with timestamps

### Tests for User Story 4

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T035 [US4] Add test `NewOrder_HasInitialPendingHistoryEntry` — Given a newly created order via POST, When GET the order, Then statusHistory has 1 entry with status "pending" in `acme-api/tests/AcmeApi.Tests/OrderStatusWorkflowTests.cs`
- [ ] T036 [US4] Add test `UpdateStatus_AppendsToHistory` — Given order transitioned pending->confirmed, When GET the order, Then statusHistory has 2 entries in chronological order in `acme-api/tests/AcmeApi.Tests/OrderStatusWorkflowTests.cs`
- [ ] T037 [US4] Add test `MultipleTransitions_HistoryContainsAllEntries` — Given order transitioned pending->confirmed->shipped->delivered, When GET the order, Then statusHistory has 4 entries with correct statuses and timestamps in `acme-api/tests/AcmeApi.Tests/OrderStatusWorkflowTests.cs`

### Implementation for User Story 4

- [ ] T038 [US4] Update `UpdateStatus` action in `acme-api/src/AcmeApi/Controllers/OrdersController.cs` to append a new `StatusHistoryEntry` with the new status and `DateTime.UtcNow` on each successful transition
- [ ] T039 [US4] Run tests and verify ALL tests pass (US1 + US2 + US3 + US4) with `dotnet test` in `acme-api/`

**Checkpoint**: Complete audit trail working. All user stories independently verifiable.

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Final verification and cleanup

- [ ] T040 Run full verify: `dotnet build --warnaserror && dotnet test` in `acme-api/`
- [ ] T041 Verify test count is at least 15 new tests in `OrderStatusWorkflowTests.cs`
- [ ] T042 Run quickstart.md validation — manually test the curl commands from `specs/001-order-status-workflow/quickstart.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — verify build
- **Foundational (Phase 2)**: Depends on Phase 1 — BLOCKS all user stories
- **US1 (Phase 3)**: Depends on Phase 2 — creates the PATCH endpoint
- **US2 (Phase 4)**: Depends on Phase 3 — adds transition guards to existing endpoint
- **US3 (Phase 5)**: Depends on Phase 3 — adds validator to existing endpoint
- **US4 (Phase 6)**: Depends on Phase 2 — adds history tracking (can run parallel with US2/US3 after US1)
- **Polish (Phase 7)**: Depends on all user stories complete

### User Story Dependencies

- **US1 (P1)**: Requires foundational models + endpoint creation. MUST be first.
- **US2 (P1)**: Requires the PATCH endpoint from US1 to exist. Sequential after US1.
- **US3 (P1)**: Requires the PATCH endpoint from US1 to exist. Can run parallel with US2.
- **US4 (P2)**: Requires foundational StatusHistory models. Can run after US1.

### Within Each User Story

- Tests MUST be written and FAIL before implementation
- Implementation follows BDD scenarios from spec.md
- Story verified before moving to next

### Parallel Opportunities

After Phase 3 (US1) completes:

```
US2 (invalid transitions) ──┐
                             ├──→ Phase 7 (Polish)
US3 (validation)       ──────┤
                             │
US4 (audit trail)      ──────┘
```

US2, US3, and US4 can theoretically run in parallel since they modify different aspects of the endpoint. However, since they all modify the same controller file, sequential execution is recommended for this project.

---

## Implementation Strategy

### MVP First (US1 Only)

1. Complete Phase 1: Setup (verify build)
2. Complete Phase 2: Foundational (models, DTOs, transition map)
3. Complete Phase 3: US1 (PATCH endpoint with valid transitions)
4. **STOP and VALIDATE**: Test the 5 valid transitions independently
5. Demo-ready with core workflow

### Full Delivery (Sequential)

1. Setup → Foundational → Foundation ready
2. US1: Valid transitions → Test → Working endpoint (MVP!)
3. US2: Invalid transitions → Test → Transition guards added
4. US3: Validation → Test → Input validation added
5. US4: Audit trail → Test → History tracking added
6. Polish → Full verify → Done

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- All tests use BDD Given-When-Then naming per constitution
- Integration tests via `WebApplicationFactory<Program>` per research decision
- Transition map is a static dictionary per research decision
- 422 for invalid transitions, 400 for validation errors per research decision
- Commit after each phase checkpoint
