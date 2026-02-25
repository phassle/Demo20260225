---
name: commit-push-pr
description: Commits, pushes, and creates a PR
disable-model-invocation: true
---

# Commit, push, and create PR

## Steps

1. Run `dotnet build --warnaserror && dotnet test` — if it fails, fix the issues first
2. Stage all relevant changes (not bin/, obj/, .env)
3. Write a commit message in imperative mood ("Add X", "Fix Y")
4. Push to the current feature branch
5. Create a PR with:
   - Title: short summary of what changed
   - Body: what was added/changed and why
   - Link to related issue if there is one
