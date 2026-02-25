---
name: review
description: Reviews code for security, tests, and conventions
argument-hint: [file-or-directory]
disable-model-invocation: true
allowed-tools: Read, Grep, Glob
---

# Code review

Review the code in $ARGUMENTS using the checklist below.

## Checklist

Refer to `examples/good-review.md` for what a good review looks like.

- [ ] All new endpoints have FluentValidation
- [ ] DTOs used — entities never exposed from API
- [ ] Tests cover happy path + edge cases
- [ ] Error responses use `{ error = "..." }` format
- [ ] No hardcoded values (use IConfiguration)
- [ ] ILogger<T> used — no Console.WriteLine
- [ ] No security issues (SQL injection, missing [Authorize], exposed secrets)
- [ ] async/await used — no .Result or .Wait()

## Output

Summarize findings: ✅ good / ⚠️ warning / ❌ must fix

End with: **Approve** / **Request changes** / **Needs discussion**.
