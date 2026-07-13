# Data Model: Order Status Workflow

## Entities

### StatusHistoryEntry (new)

| Field | Type | Description |
|-------|------|-------------|
| Status | string | The status value at this point in time |
| ChangedAt | DateTime | UTC timestamp when the status changed |

### OrderEntity (modified)

| Field | Type | Change |
|-------|------|--------|
| Id | Guid | Existing |
| CustomerEmail | string | Existing |
| Items | List\<OrderItem\> | Existing |
| Status | string | Existing |
| CreatedAt | DateTime | Existing |
| StatusHistory | List\<StatusHistoryEntry\> | **New** — audit trail of all status changes |

### UpdateOrderStatusRequest (new)

| Field | Type | Validation |
|-------|------|------------|
| Status | string | NotEmpty, must be one of: pending, confirmed, shipped, delivered, cancelled |

## DTOs

### StatusHistoryEntryDto (new)

| Field | Type |
|-------|------|
| Status | string |
| ChangedAt | DateTime |

### OrderDto (modified)

| Field | Type | Change |
|-------|------|--------|
| Id | Guid | Existing |
| CustomerEmail | string | Existing |
| Items | List\<OrderItemDto\> | Existing |
| Status | string | Existing |
| CreatedAt | DateTime | Existing |
| StatusHistory | List\<StatusHistoryEntryDto\> | **New** |

## State Transitions

```
pending ──→ confirmed ──→ shipped ──→ delivered
  │              │
  └──→ cancelled ←┘
```

**Allowed transitions:**

| From | To |
|------|----|
| pending | confirmed |
| pending | cancelled |
| confirmed | shipped |
| confirmed | cancelled |
| shipped | delivered |

**Terminal states** (no transitions out): `delivered`, `cancelled`

**Self-transitions**: Not allowed (pending -> pending is rejected)
