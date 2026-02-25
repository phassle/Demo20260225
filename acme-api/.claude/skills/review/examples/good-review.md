# Example: Good review output

## Review of Controllers/ProductsController.cs

✅ **FluentValidation** — All input validated via IValidator<CreateProductRequest>
✅ **DTOs** — Entities never exposed. ProductDto used in responses
✅ **Error handling** — Consistent `{ error = "..." }` format, correct status codes
⚠️ **Tests** — Happy path covered, but missing edge case for empty name
❌ **Hardcoded** — Port number hardcoded, should be in appsettings.json

### Verdict: **Request changes**
Fix the hardcoded port and add the missing edge case test.
