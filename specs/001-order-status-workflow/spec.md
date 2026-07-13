# Feature Specification: Order Status Workflow

**Feature Branch**: `001-order-status-workflow`
**Created**: 2026-02-26
**Status**: Draft
**Input**: GitHub Issue #4 — Add order status workflow with PATCH endpoint

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Update Order Status (Priority: P1)

As a store operator, I want to advance an order through its lifecycle (pending to confirmed to shipped to delivered) so that customers and internal systems can track order progress.

**Why this priority**: This is the core feature — without status transitions, the entire workflow has no value. It delivers the PATCH endpoint and transition validation.

**Independent Test**: Can be fully tested by creating an order and transitioning it through valid states. Delivers the complete happy path for order lifecycle management.

**Acceptance Scenarios**:

1. **Given** an order exists with status "pending", **When** I request status change to "confirmed", **Then** the order status is updated and the updated order is returned with status 200
2. **Given** an order exists with status "confirmed", **When** I request status change to "shipped", **Then** the order status is updated and the updated order is returned with status 200
3. **Given** an order exists with status "shipped", **When** I request status change to "delivered", **Then** the order status is updated and the updated order is returned with status 200
4. **Given** an order exists with status "pending", **When** I request status change to "cancelled", **Then** the order status is updated and the updated order is returned with status 200
5. **Given** an order exists with status "confirmed", **When** I request status change to "cancelled", **Then** the order status is updated and the updated order is returned with status 200

---

### User Story 2 - Reject Invalid Transitions (Priority: P1)

As a store operator, I want the system to reject invalid status transitions so that orders cannot enter inconsistent states (e.g., going from "delivered" back to "pending").

**Why this priority**: Equal to US1 — without transition guards, the workflow is unreliable. Invalid transitions must be blocked from day one.

**Independent Test**: Can be tested by attempting all invalid transitions and verifying each is rejected with a descriptive error.

**Acceptance Scenarios**:

1. **Given** an order exists with status "shipped", **When** I request status change to "pending", **Then** the system rejects with status 422 and an error message "Cannot transition from 'shipped' to 'pending'"
2. **Given** an order exists with status "delivered", **When** I request status change to "confirmed", **Then** the system rejects with status 422 and a descriptive error
3. **Given** an order exists with status "cancelled", **When** I request status change to "pending", **Then** the system rejects with status 422 and a descriptive error
4. **Given** an order exists with status "delivered", **When** I request status change to any other status, **Then** the system rejects — delivered is a terminal state
5. **Given** an order exists with status "cancelled", **When** I request status change to any other status, **Then** the system rejects — cancelled is a terminal state

---

### User Story 3 - Validate Status Input (Priority: P1)

As a store operator, I want the system to validate the status value in my request so that typos and invalid values are caught before processing.

**Why this priority**: Input validation is a prerequisite for safe operation — all input must go through FluentValidation per the constitution.

**Independent Test**: Can be tested by sending invalid payloads (empty status, unknown status strings) and verifying 400 responses.

**Acceptance Scenarios**:

1. **Given** an order exists, **When** I send a request with an empty status field, **Then** the system returns 400 with a validation error
2. **Given** an order exists, **When** I send a request with status "unknown", **Then** the system returns 400 with a validation error
3. **Given** I provide a non-existent order ID, **When** I request any status change, **Then** the system returns 404 with error "Order not found"

---

### User Story 4 - Audit Trail (Priority: P2)

As a store operator, I want a history of all status changes on an order so that I can review when and how an order progressed through the workflow.

**Why this priority**: This is a stretch goal that adds observability but is not required for the core status transition feature to function.

**Independent Test**: Can be tested by transitioning an order through multiple states and verifying the history list grows with correct timestamps and statuses.

**Acceptance Scenarios**:

1. **Given** a newly created order, **When** I view the order, **Then** the status history contains one entry for the initial "pending" status
2. **Given** an order transitioned from "pending" to "confirmed", **When** I view the order, **Then** the status history contains two entries in chronological order
3. **Given** an order transitioned through multiple states, **When** I view the order, **Then** each history entry includes the status and the timestamp of the change

---

### Edge Cases

- What happens when the same status is requested twice (e.g., "pending" to "pending")? Treated as an invalid transition — no self-transitions allowed.
- What happens when the request body is missing entirely? FluentValidation returns 400.
- What happens with concurrent status updates to the same order? In-memory store handles last-write-wins (acceptable for demo project).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST expose a PATCH endpoint at `/api/orders/{id}/status` that accepts a status string in the request body
- **FR-002**: System MUST validate that the requested status is one of: `pending`, `confirmed`, `shipped`, `delivered`, `cancelled`
- **FR-003**: System MUST validate that the status field is not empty
- **FR-004**: System MUST enforce the allowed transition graph: pending->confirmed, pending->cancelled, confirmed->shipped, confirmed->cancelled, shipped->delivered
- **FR-005**: System MUST return 422 with `{ error: "Cannot transition from '{current}' to '{requested}'" }` for invalid transitions
- **FR-006**: System MUST return 404 with `{ error: "Order not found" }` when the order ID does not exist
- **FR-007**: System MUST return 200 with the updated order DTO on successful transition
- **FR-008**: System MUST maintain a status history list on each order recording each status and when it changed
- **FR-009**: System MUST include the status history in the order DTO response
- **FR-010**: System MUST record the initial "pending" status in the history when an order is created

### Key Entities

- **Order Status**: The current lifecycle state of an order. One of: pending, confirmed, shipped, delivered, cancelled.
- **Status Transition**: A directed edge in the workflow graph defining which status changes are allowed.
- **Status History Entry**: A record of a status change, containing the status value and the timestamp when the change occurred.
- **Update Status Request**: The input payload containing the desired new status.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: All 5 valid status transitions succeed and return the updated order
- **SC-002**: All invalid transitions (every combination not in the allowed graph) are rejected with descriptive errors
- **SC-003**: Unknown order IDs return 404 in all cases
- **SC-004**: Invalid input (empty status, unknown status values) is caught by validation before reaching business logic
- **SC-005**: Status history accurately records every transition with timestamps
- **SC-006**: At least 15 test cases cover all valid transitions, all invalid transitions, 404, validation errors, and audit trail
- **SC-007**: `dotnet build --warnaserror && dotnet test` passes with zero failures and zero warnings
