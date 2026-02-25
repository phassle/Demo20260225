---
name: simplify
description: Simplifies code by removing over-engineering and improving readability (Boris Simplify phase)
argument-hint: [file-or-directory]
disable-model-invocation: true
---

# Simplify phase — Boris workflow

Review and simplify: $ARGUMENTS

## Checklist

Go through each item. Fix what you find:

1. **Duplication** — Any copy-paste code? Extract shared logic.
2. **Naming** — Are names clear? Could a new team member understand them?
3. **Complexity** — Any over-engineered abstractions? Simplify.
4. **Dead code** — Unused imports, methods, variables? Remove them.
5. **Comments** — Comments explaining "what" instead of "why"? Remove or rewrite.
6. **Error handling** — Consistent? Following project patterns?

## Rules

- All tests MUST stay green after every change
- Run `dotnet build --warnaserror && dotnet test` after simplifying
- Make small, focused changes — one simplification at a time
- If in doubt, leave it. Simplicity > cleverness.
