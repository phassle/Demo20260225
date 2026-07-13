# Contract: PATCH /api/orders/{id}/status

## Request

**Method**: PATCH
**Route**: `/api/orders/{id}/status`
**Path parameter**: `id` (Guid) — the order ID

**Body**:
```json
{
  "status": "confirmed"
}
```

## Responses

### 200 OK — Successful transition

```json
{
  "id": "d4e5f6a7-b8c9-0123-defa-234567890123",
  "customerEmail": "alice@example.com",
  "items": [
    { "productId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "quantity": 1 }
  ],
  "status": "confirmed",
  "createdAt": "2026-02-20T10:00:00Z",
  "statusHistory": [
    { "status": "pending", "changedAt": "2026-02-20T10:00:00Z" },
    { "status": "confirmed", "changedAt": "2026-02-26T14:30:00Z" }
  ]
}
```

### 400 Bad Request — Validation error (empty/invalid status string)

```json
{
  "error": "Status must be one of: pending, confirmed, shipped, delivered, cancelled"
}
```

### 404 Not Found — Order does not exist

```json
{
  "error": "Order with id d4e5f6a7-b8c9-0123-defa-234567890123 not found"
}
```

### 422 Unprocessable Entity — Invalid state transition

```json
{
  "error": "Cannot transition from 'delivered' to 'pending'"
}
```
