---
name: save-plan
description: Save or update a plan as a GitHub Issue. Creates a new issue or updates an existing one with phase checkboxes.
disable-model-invocation: true
allowed-tools: Bash(gh *)
---

# Save / update plan on GitHub Issue

## If no issue number is given ($ARGUMENTS is empty or a feature description)

Create a NEW GitHub Issue from the current conversation's plan:
1. Extract phases, acceptance criteria, file changes from conversation
2. Create issue: `gh issue create --title "..." --body "..." --label plan,ai-generated`
3. Report the issue number and URL

## If an issue number is given ($ARGUMENTS starts with #)

UPDATE the existing issue with current progress:
1. Read the existing issue: `gh issue view $ARGUMENTS`
2. Update the body with current phase status:
   - `- [x]` for completed phases
   - `- [ ]` for remaining phases
   - Add notes on what was done per phase
3. Update: `gh issue edit $ARGUMENTS --body "..."`
4. Add a comment summarizing what changed: `gh issue comment $ARGUMENTS --body "Phase N completed: ..."`

## Issue format

```markdown
## Overview
[1-2 sentences describing the feature]

## Phases
- [x] Phase 1: Entity + DTO + FluentValidation validator
  - Files: src/AcmeApi/Models/Discount.cs, src/AcmeApi/Validators/CreateDiscountValidator.cs
  - Acceptance: Validator handles percentage/fixed types
- [ ] Phase 2: Controller + endpoints
  - Files: src/AcmeApi/Controllers/DiscountsController.cs
  - Acceptance: POST/GET/DELETE endpoints work
- [ ] Phase 3: Tests (TDD)
  - Acceptance: Happy path + edge cases + error cases

## Acceptance criteria
[Overall criteria for the feature to be considered done]
```

## Rules

- Always preserve existing comments and discussion on the issue
- Mark completed phases with `- [x]`, include a brief note on what was done
- The issue should be detailed enough for ANY agent (or human) to pick up remaining work
