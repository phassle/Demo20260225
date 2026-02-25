---
name: plan-feature
description: Creates a multi-phase implementation plan for a new feature (Boris Plan phase)
argument-hint: [feature-description]
disable-model-invocation: true
---

# Plan phase — Boris workflow

Create a detailed implementation plan for: $ARGUMENTS

## Steps

1. **Read** CLAUDE.md and .claude/rules/ to understand the project
2. **Analyze** the existing codebase — which files need to change?
3. **Ask clarifying questions** if requirements are ambiguous
4. **Break into phases** (max 3-5 phases), each completable in one session:
   - Phase 1: Entity + DTO + FluentValidation validator
   - Phase 2: Controller + endpoints
   - Phase 3: Tests (TDD — use /generate-tests)
   - Phase 4: Error handling + edge cases
   - Phase 5: Simplify + final review

For each phase, list:
- Files to create/modify
- Acceptance criteria
- Test requirements

## Output
Present the plan as a numbered checklist. Wait for approval before implementing.
