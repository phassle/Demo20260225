---
name: generate-tests
description: Generates tests using TDD red-green-refactor (Boris Implement phase)
argument-hint: [file-path]
---

# Generate tests — TDD process

Generate tests for: $ARGUMENTS

## Process — one test at a time

For each behavior:

1. **RED** — Write ONE failing test. Run `dotnet test`. Confirm it fails.
2. **GREEN** — Write the minimal code to make that ONE test pass. Run `dotnet test`. Confirm green.
3. Repeat for the next behavior.

## Rules

- Only ONE test at a time. Do not write multiple tests before implementing.
- Each test should verify one specific behavior.
- Do not modify tests to make them pass — modify the implementation.
- Use xUnit (with [Fact] and [Theory]) + FluentAssertions
- Place tests in tests/AcmeApi.Tests/
- Cover: happy path + at least 2 edge cases + 1 error case
- Use descriptive test names: "Create_WithEmptyName_ShouldFail", not "Test3"

## After all behaviors are implemented

- Run `dotnet build --warnaserror && dotnet test` to confirm everything passes
- Look for refactor candidates: duplication, unclear names, unnecessary complexity
