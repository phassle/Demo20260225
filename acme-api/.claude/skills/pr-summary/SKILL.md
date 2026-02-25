---
name: pr-summary
description: Summarize the current pull request with diff and comments
context: fork
agent: Explore
allowed-tools: Bash(gh *)
---

# PR Summary

## Current PR context
- Diff: !`gh pr diff`
- Comments: !`gh pr view --comments`
- Changed files: !`gh pr diff --name-only`

Summarize this pull request:
1. What changed and why?
2. Key files modified
3. Any outstanding review comments?
4. Risk assessment: low / medium / high
