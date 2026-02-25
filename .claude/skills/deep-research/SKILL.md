---
name: deep-research
description: Researches a topic by reading the codebase thoroughly, returns a structured summary
argument-hint: [topic-or-question]
context: fork
agent: Explore
---

# Deep research

Research the following topic in this codebase: $ARGUMENTS

## Process

1. Search the entire codebase for relevant files
2. Read and understand the implementation
3. Trace data flow and dependencies
4. Identify patterns and conventions used

## Output

Return a structured summary:
- **Overview** — what exists today
- **Key files** — list the most relevant files
- **Patterns** — conventions and patterns used
- **Gaps** — what's missing or could be improved
