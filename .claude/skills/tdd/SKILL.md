---
name: tdd
description: Red-green-refactor development. Activates when building new features, endpoints, or fixing bugs. Enforces one-test-at-a-time incremental loop.
---

# TDD — Red Green Refactor

Use this approach for every new feature or bug fix.

## Incremental loop

For each behavior to implement:

1. **RED** — Write ONE failing test. Run `dotnet test`. Confirm it fails.
2. **GREEN** — Write the minimal code to make that ONE test pass. Run `dotnet test`. Confirm green.
3. Repeat for the next behavior.

## Rules

- Only ONE test at a time. Do not write multiple tests before implementing.
- Each test should verify one specific behavior.
- Do not modify tests to make them pass — modify the implementation.
- Run `dotnet test` after every step to confirm red → green.

## After all behaviors are implemented

- Look for refactor candidates: duplication, unclear names, unnecessary complexity.
- Refactor while keeping all tests green.
- Run `dotnet build --warnaserror && dotnet test` to confirm everything passes.
