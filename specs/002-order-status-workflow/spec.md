# Feature Specification: Order Status Workflow

**Feature Branch**: `002-order-status-workflow`
**Created**: 2026-02-26
**Status**: Draft
**Input**: GitHub Issue #4 — Add order status workflow with PATCH endpoint

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Update Order Status (Priority: P1)

As a shop administrator, I want to move an order through its lifecycle (pending to confirmed to shipped to delivered) so that customers and internal systems can track order progress.

**Why this priority**: This is the core functionality — without status transitions, orders remain static and the workflow has no value.

**Independent Test**: Can be fully tested by creating an order, then updating its status through valid transitions, verifying the response contains the updated status at each step.

**Acceptance Scenarios**:

1. **Given** an order exists with status "pending", **When** a status update to "confirmed" is requested, **Then** the order status changes to "confirmed" and the updated order is returned with status 200.
2. **Given** an order exists with status "confirmed", **When** a status update to "shipped" is requested, **Then** the order status changes to "shipped" and the updated order is returned with status 200.
3. **Given** an order exists with status "shipped", **When** a status update to "delivered" is requested, **Then** the order status changes to "delivered" and the updated order is returned with status 200.

---

### User Story 2 - Cancel an Order (Priority: P1)

As a shop administrator, I want to cancel an order that has not yet been shipped so that mistaken or fraudulent orders can be stopped before fulfillment.

**Why this priority**: Cancellation is equally critical to the happy path — without it, incorrect orders cannot be stopped.

**Independent Test**: Can be fully tested by creating an order, then cancelling it from "pending" or "confirmed" status, verifying the response reflects cancellation.

**Acceptance Scenarios**:

1. **Given** an order exists with status "pending", **When** a status update to "cancelled" is requested, **Then** the order status changes to "cancelled" and the updated order is returned with status 200.
2. **Given** an order exists with status "confirmed", **When** a status update to "cancelled" is requested, **Then** the order status changes to "cancelled" and the updated order is returned with status 200.

---

### User Story 3 - Reject Invalid Transitions (Priority: P1)

As a system, I must enforce the status workflow rules so that orders cannot be moved into illogical states (e.g., delivering a cancelled order, going backwards from shipped to pending).

**Why this priority**: Without enforcement, the workflow is meaningless — any status could be set at any time.

**Independent Test**: Can be fully tested by attempting invalid transitions and verifying each is rejected with an appropriate error.

**Acceptance Scenarios**:

1. **Given** an order exists with status "shipped", **When** a status update to "cancelled" is requested, **Then** the request is rejected with a 422 status and an error message explaining the transition is not allowed.
2. **Given** an order exists with status "delivered", **When** any status update is requested, **Then** the request is rejected with a 422 status because delivered is a terminal state.
3. **Given** an order exists with status "cancelled", **When** any status update is requested, **Then** the request is rejected with a 422 status because cancelled is a terminal state.
4. **Given** an order exists with status "pending", **When** a status update to "shipped" is requested, **Then** the request is rejected with a 422 status because orders must be confirmed before shipping.

---

### User Story 4 - Validate Status Input (Priority: P2)

As a system, I must validate the status update request so that empty, missing, or unrecognized status values are rejected before reaching business logic.

**Why this priority**: Input validation prevents garbage data and provides clear error messages, but is secondary to the core workflow logic.

**Independent Test**: Can be fully tested by sending requests with empty, null, or invalid status values and verifying each returns a 400 validation error.

**Acceptance Scenarios**:

1. **Given** a valid order exists, **When** a status update with an empty string is requested, **Then** the request is rejected with a 400 status and a validation error.
2. **Given** a valid order exists, **When** a status update with an unrecognized value (e.g., "processing") is requested, **Then** the request is rejected with a 400 status and a validation error.

---

### User Story 5 - Status Change Audit Trail (Priority: P3)

As a shop administrator, I want to see the history of all status changes on an order so that I can audit when each transition happened for customer service and dispute resolution.

**Why this priority**: Audit trail is a stretch goal that adds operational value but is not required for the core status workflow to function.

**Independent Test**: Can be fully tested by updating an order's status multiple times and verifying the order response includes a chronological history of all status changes with timestamps.

**Acceptance Scenarios**:

1. **Given** an order has transitioned through multiple statuses, **When** the order is retrieved, **Then** the response includes a status history showing each status and when the change occurred, in chronological order.
2. **Given** a newly created order, **When** the order is retrieved, **Then** the status history contains exactly one entry for the initial "pending" status.

---

### Edge Cases

- What happens when a status update is requested for a non-existent order ID? System returns 404 with descriptive error.
- What happens when the request body is missing or malformed? System returns 400 with validation error.
- What happens when the requested status is the same as the current status? This is treated as an invalid transition (no self-transitions allowed) and returns 422.
- What happens with case sensitivity in status values? Status values are case-insensitive (e.g., "Confirmed" and "confirmed" are treated the same).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST provide a PATCH endpoint for updating order status at the order resource path with a status sub-resource.
- **FR-002**: System MUST accept a request body containing the target status value.
- **FR-003**: System MUST validate that the status value is not empty.
- **FR-004**: System MUST validate that the status value is one of the recognized statuses: pending, confirmed, shipped, delivered, cancelled.
- **FR-005**: System MUST enforce the following valid transitions:
  - pending to confirmed
  - pending to cancelled
  - confirmed to shipped
  - confirmed to cancelled
  - shipped to delivered
- **FR-006**: System MUST reject any transition not listed in FR-005 with a 422 status and an error message stating which transition was attempted.
- **FR-007**: System MUST return 404 when the specified order does not exist.
- **FR-008**: System MUST return the full updated order representation on successful status change with a 200 status.
- **FR-009**: System MUST record each status change with a timestamp in a status history on the order.
- **FR-010**: System MUST include the status history when returning order data (both individual and list endpoints).
- **FR-011**: System MUST treat status values as case-insensitive.

### Key Entities

- **Order**: Extended with a status history — a chronological list of status change entries recording which status was set and when.
- **Status Change Entry**: Represents a single status transition event, containing the status value and the timestamp of the change.
- **Status Update Request**: The input for the PATCH endpoint, containing the desired target status.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: All 5 valid status transitions complete successfully and return the updated order.
- **SC-002**: All invalid transitions (at least 10 combinations) are rejected with descriptive error messages.
- **SC-003**: Input validation catches empty and unrecognized status values before business logic executes.
- **SC-004**: Non-existent orders produce a clear "not found" response.
- **SC-005**: Each status change is recorded with a timestamp and the full history is visible on the order.
- **SC-006**: At least 15 test cases cover all valid transitions, invalid transitions, validation errors, not-found scenarios, and audit trail behavior.
- **SC-007**: All existing functionality continues to work — no regressions in order creation, retrieval, or listing.

## Assumptions

- Status values are stored as strings (not numeric codes).
- The initial status for all new orders is "pending".
- Timestamps for the audit trail use UTC.
- Self-transitions (e.g., pending to pending) are not allowed.
- The audit trail is initialized when an order is created, with the first entry recording the "pending" status.
