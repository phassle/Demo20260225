# Workshop Demo — Acme Webshop API (.NET 8)

Demo project for the **Agentic Development** workshop (Viedoc, 2026-02-25).
Everything you need is in `acme-api/`.

## Quick setup

```bash
cd acme-api
dotnet restore
dotnet build --warnaserror         # should compile clean
dotnet test                        # should pass 7+ tests
dotnet run --project src/AcmeApi   # starts on localhost:5000
```

Open http://localhost:5000/swagger for the interactive API docs.

## Issue-first workflow — Plan → Implement → Simplify → Verify

Start from an existing GitHub Issue, plan it, work phase by phase, and update the issue as you go:

| Step | Command | What happens |
|------|---------|-------------|
| **Pick up** | `"Get issue #12 and plan it"` | Agent reads issue + CLAUDE.md + codebase |
| **Save plan** | `/save-plan #12` | Writes phases back to issue as checkboxes |
| **Implement** | `/generate-tests discount codes` | TDD: RED → GREEN → repeat, `/save-plan #12` after each phase |
| **Simplify** | `/simplify src/AcmeApi/Controllers/DiscountsController.cs` | Remove over-engineering, improve names |
| **Verify** | `dotnet build --warnaserror && dotnet test` | Build + test (also runs on Stop hook) |
| **Done** | `/save-plan #12` | Mark all phases done, hand off or ship |

## Three layers of control

```
Tests              →  define WHAT can be done     (deterministic)
CLAUDE.md          →  tell WHEN to do it          (guidance)
Hooks / CI         →  FORCE it to happen          (automatic)
```

| Layer | File | What it does |
|-------|------|-------------|
| Tests | `dotnet test` | xUnit + FluentAssertions |
| CLAUDE.md | "Run `dotnet build && dotnet test` before every commit" | Agent reads and follows |
| Hook (Stop) | `dotnet build --warnaserror && dotnet test` on every task completion | Even if agent "forgets" |
| Hook (PostToolUse) | `dotnet format` after every Write/Edit | Instant format on change |

## File structure

```
acme-api/
├── CLAUDE.md                         ← Agent's onboarding (Boris workflow + rules)
├── AcmeApi.sln                       ← Solution file
├── .claude/
│   ├── skills/                       ← Skills (with frontmatter)
│   │   ├── plan-feature/SKILL.md     ← /plan-feature — Boris Plan phase
│   │   ├── generate-tests/SKILL.md   ← /generate-tests — Boris Implement phase
│   │   ├── simplify/SKILL.md         ← /simplify — Boris Simplify phase
│   │   ├── review/                   ← /review — with examples/ subfolder
│   │   │   ├── SKILL.md
│   │   │   └── examples/good-review.md
│   │   ├── commit-push-pr/SKILL.md   ← /commit-push-pr — ship it
│   │   ├── tdd/SKILL.md              ← Auto-activates on new features
│   │   ├── pr-review/SKILL.md        ← Auto-activates on PR discussions
│   │   ├── pr-summary/SKILL.md       ← Dynamic context: !`gh pr diff`
│   │   ├── save-plan/SKILL.md        ← Save plan to GitHub Issue
│   │   └── deep-research/SKILL.md    ← Sub-agent: context: fork + agent: Explore
│   ├── agents/
│   │   ├── planner.md                ← Creates GitHub Issues from plans
│   │   ├── simplifier.md             ← Simplify phase: refactor + clean up
│   │   └── reviewer.md               ← Verify phase reviewer
│   ├── rules/                        ← Auto-loaded reference docs
│   │   ├── architecture.md
│   │   └── testing.md
│   └── settings.json                 ← Hooks + permissions (deny list)
├── src/AcmeApi/
│   ├── Program.cs                    ← Entry point + DI config
│   ├── Controllers/                  ← API controllers (Products, Orders — Health created in demo)
│   ├── Models/                       ← Entities + DTOs + in-memory stores
│   └── Validators/                   ← FluentValidation validators
└── tests/AcmeApi.Tests/              ← xUnit + FluentAssertions tests
```

## What to demo

### Workshop 1 — Meet your new colleague
| What to show | How |
|-------------|-----|
| First conversation | `cd acme-api && claude` → "Add a health check endpoint at GET /api/health" |
| Plan mode | Shift+Tab × 2 → "Add a DELETE endpoint for products" |
| Bug fix | "Bug: POST /api/orders accepts empty Items list. Should require at least one item." |

### Workshop 2 — Onboard her
| What to show | How |
|-------------|-----|
| Without CLAUDE.md | Rename CLAUDE.md → "Add GET /api/customers" (she guesses wrong stack) |
| With CLAUDE.md | Restore → same task (now follows .NET patterns) |
| Write your own | "Analyze this codebase and create an AGENTS.md file..." |

### Workshop 3 — Train her
| What to show | How |
|-------------|-----|
| Rich skill folder | Show `.claude/skills/review/` — SKILL.md + examples/ |
| Frontmatter | Show `disable-model-invocation: true` in plan-feature |
| Sub-agent skill | Show `context: fork` + `agent: Explore` in deep-research |
| PostToolUse hook | Edit a file → `dotnet format` runs automatically |
| Stop hook | Agent finishes → `dotnet build && dotnet test` runs |

### Workshop 4 — Build a team
| What to show | How |
|-------------|-----|
| Planner agent | `claude --agent planner "Add discount codes"` |
| Reviewer agent | `claude --agent reviewer` after changes |
| Parallel agents | 3 terminals with different tasks |
| Boris workflow | `/plan-feature` → `/generate-tests` → `/simplify` → verify |

## Example prompts

### Simple task
```
Add a DELETE endpoint for products by ID. Return 404 if not found, 204 on success.
```

### Boris workflow — full cycle
```
/plan-feature Add discount codes with percentage and fixed amount types, plus expiry dates
```

### Bug fix
```
Bug: POST /api/orders accepts an empty Items list.
It should require at least one item.
```

### Multi-phase plan
```
Add a full customer management system: CRUD endpoints, search by email,
order history per customer, and a stats endpoint showing top customers.
```

## Known bugs (for demo!)

- **POST /api/orders accepts empty Items list** — `CreateOrderValidator` uses `.NotNull()` without `.MinimumLength(1)`. Used during the bug fix demo.
