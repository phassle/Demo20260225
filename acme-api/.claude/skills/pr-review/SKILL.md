---
name: pr-review
description: Reviews pull requests according to the team's code review checklist. Activates when discussing PRs, code review, or merge requests.
---

# PR Review Skill

Review the PR using our team checklist:

## Checklist
- [ ] All new endpoints have FluentValidation
- [ ] DTOs used — entities never exposed from API
- [ ] Tests cover happy path + edge cases
- [ ] Error responses use `{ error = "..." }` format
- [ ] No hardcoded values (use IConfiguration)
- [ ] ILogger<T> used — no Console.WriteLine
- [ ] Commit messages in imperative mood
- [ ] async/await — no .Result or .Wait()

## Output format
Give a summary with ✅ / ⚠️ / ❌ for each item.
End with: "Approve" / "Request changes" / "Needs discussion".
