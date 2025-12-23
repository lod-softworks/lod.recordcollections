# AGENTS

This document defines how automated agents (such as Cursor, GitHub Copilot, or other AI tools) should behave when making changes in this repository.

## Scope of Work

- Stay **focused on the user’s explicit request**.
- Keep code changes to the **minimum required** to complete the requested work.
- Do **not** perform unrelated cleanup, refactors, or “drive-by” improvements.
- If additional enhancements or changes are identified, **suggest them separately** instead of implementing them automatically.
- Ask clarifying questions when requirements or scope are ambiguous.
- Aim for diffs that are **easy to read and review**, with clear intent behind every change.

## Code Style and Design

- Use **meaningful, descriptive names**:
  - Avoid unclear abbreviations.
  - Prefer pronounceable, consistent naming.
- Keep functions **small** and single-purpose:
  - Avoid flag parameters and side effects.
  - Maintain a single level of abstraction per function.
- Apply **Single Responsibility Principle** to classes and functions.
- Maintain **clean formatting**:
  - Consistent indentation and spacing.
  - Blank lines where needed for readability.
- Minimize comments:
  - Write self-explanatory code.
  - Use comments only for complex logic or public APIs that need extra context.

## Error Handling

- Prefer **exceptions over return codes**.
- Avoid catching generic exceptions (for example, avoid `catch (Exception)` when possible).
- Fail fast and handle exceptions at a **high level** in the application.

## Avoiding Duplication and Code Smells

- Extract shared logic into reusable functions or classes (**DRY** – Don’t Repeat Yourself).
- Be cautious of:
  - Long functions
  - Large classes
  - Deep nesting
  - Primitive obsession
  - Long parameter lists
  - Magic numbers or strings
  - Inconsistent naming

## Git Committing

- Prefer **small, focused commits** that are easy to understand, review, and diff.
- Use **Conventional Commits** style messages for all commits (for example: `feat:`, `fix:`, `chore:`, `docs:`, `refactor:`, etc.).
- Create **`wip` commits** when helpful to show incremental progress during longer sessions.

## Pull Requests

- Ensure all changes **adhere to the coding guidelines** above.
- Keep pull requests **scoped and reviewable**, avoiding unrelated refactors.
- When summarizing changes, use a **strict but constructive tone**, and:
  - Use bullet points to list issues or improvements.
  - Provide clear alternatives and improved code suggestions when relevant.
