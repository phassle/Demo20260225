# Quickstart: Order Status Workflow

## Prerequisites

- .NET 8 SDK
- Clone the repo and checkout branch `001-order-status-workflow`

## Run the API

```bash
dotnet run --project acme-api/src/AcmeApi
```

Swagger UI available at: http://localhost:5000/swagger (or the port shown in console)

## Try the workflow

### 1. Create an order

```bash
curl -X POST http://localhost:5000/api/orders \
  -H "Content-Type: application/json" \
  -d '{"customerEmail": "test@example.com", "items": [{"productId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "quantity": 1}]}'
```

Copy the `id` from the response.

### 2. Confirm the order

```bash
curl -X PATCH http://localhost:5000/api/orders/{id}/status \
  -H "Content-Type: application/json" \
  -d '{"status": "confirmed"}'
```

### 3. Ship the order

```bash
curl -X PATCH http://localhost:5000/api/orders/{id}/status \
  -H "Content-Type: application/json" \
  -d '{"status": "shipped"}'
```

### 4. Deliver the order

```bash
curl -X PATCH http://localhost:5000/api/orders/{id}/status \
  -H "Content-Type: application/json" \
  -d '{"status": "delivered"}'
```

### 5. Try an invalid transition (should return 422)

```bash
curl -X PATCH http://localhost:5000/api/orders/{id}/status \
  -H "Content-Type: application/json" \
  -d '{"status": "pending"}'
```

### 6. Check the audit trail

```bash
curl http://localhost:5000/api/orders/{id}
```

The `statusHistory` array shows all transitions with timestamps.

## Run tests

```bash
dotnet test acme-api/
```

## Verify (build + tests)

```bash
dotnet build --warnaserror && dotnet test acme-api/
```
