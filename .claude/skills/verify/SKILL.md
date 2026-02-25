---
name: verify
description: Runs build, tests, and code review as the final quality gate (Boris Verify phase)
argument-hint: [file-or-directory]
---

# Verify phase — Boris workflow

Verify the quality of recent changes: $ARGUMENTS

## Steps — run in order

### 1. Build
```bash
dotnet build --warnaserror
```
If warnings or errors exist, fix them before continuing.

### 2. Tests
```bash
dotnet test
```
All tests must pass. If any fail, fix the failing tests or implementation before continuing.

### 3. Code review

Check all changed files against the project checklist:

- [ ] All new endpoints have FluentValidation validators
- [ ] DTOs used — entities never exposed from API
- [ ] Tests cover happy path + at least 2 edge cases + 1 error case
- [ ] Error responses use `{ error = "..." }` format with correct status codes
- [ ] No hardcoded values (use IConfiguration)
- [ ] ILogger<T> used — no Console.WriteLine
- [ ] No security issues (SQL injection, missing [Authorize], exposed secrets)
- [ ] async/await used — no .Result or .Wait()
- [ ] Naming follows conventions: PascalCase for classes/methods, descriptive names

### 4. Summary

Report results:

| Check       | Status |
|-------------|--------|
| Build       | ✅ / ❌ |
| Tests       | ✅ / ❌ (X passed, Y failed) |
| Code review | ✅ / ⚠️ / ❌ |

List any issues found with ⚠️ warnings or ❌ must-fix items.

End with: **Ready to ship** or **Needs fixes** (with specific action items).
