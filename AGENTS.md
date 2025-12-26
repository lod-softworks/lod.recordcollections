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

## Record Collection File Layout

- Implement `RecordDictionary`, `RecordList`, `RecordSet`, and any future record collections as `partial` types.
- Keep constructors and core state in `<Type>.cs`, record-specific overrides in `<Type>.Record.cs`, and isolate each interface contract into its own `<Type>.<Interface>.cs` file (for example: `RecordList.IEqualityComparer.cs`).
- When adding a new interface to a collection, introduce or update the corresponding partial file instead of expanding the base class.
- Use the existing `RecordDictionary` split as the authoritative reference for how the files should nest and be named.

## Achieving Record State

Record Collections achieve `record` semantics through a combination of inheritance, manual method overrides, and post-compilation IL modification:

1. **Inheritance**: Each collection inherits from its corresponding base type (e.g., `RecordList<T>` inherits from `List<T>`, `RecordDictionary<TKey, TValue>` inherits from `Dictionary<TKey, TValue>`).

2. **Manual Record Contract Implementation**: The `<Type>.Record.cs` file implements the record specification by defining and overriding the following methods (as seen in `RecordList.Record.cs`):
   - `EqualityContract` - protected virtual property returning the collection's type
   - Protected constructor `RecordType(RecordType original)` - used for cloning via the protected record constructor pattern
   - `GetHashCode()` - overridden to delegate to the collection's `Comparer`
   - `Equals(object? obj)` - overridden to delegate to the collection's `Comparer`
   - `Equals(BaseType other)` - public non-virtual method (e.g., `Equals(List<T> other)`)
   - `Equals(RecordType? other)` - public virtual method (e.g., `Equals(RecordList<T>? other)`)
   - `PrintMembers(StringBuilder builder)` - protected virtual method for record printing
   - `operator ==` and `operator !=` - static operators for equality comparison

3. **IL Assembler Post-Compilation**: The `Lod.RecordCollections.IlAssembler` project adds the `<Clone>$` method via IL modification after the main library is compiled. This method is required by the C# record specification but cannot be directly implemented in C#. The IL assembler:
   - Decompiles the compiled assembly to IL
   - Injects the `<Clone>$` method IL for each collection type
   - Recompiles the modified IL back to the assembly

This approach allows the collections to behave as `record class` types while inheriting from their base collection types, enabling drop-in replacement functionality.

## Comparers and Interface Implementations

- All equality, comparison, and interface implementations **must directly delegate to the collection’s `Comparer`** (the `IRecordCollectionComparer` instance) and **must not**:
  - Call other helper methods on the same type,
  - Contain additional in-class comparison logic,
  - Or depend on inheritance/overrides to “fix up” behavior.
- This rule applies to all equality/comparison-related interfaces, including (but not limited to): `IEquatable<T>`, `IComparable<T>`, `IEqualityComparer`, `IEqualityComparer<T>`, `IComparer`, `IComparer<T>`, `IStructuralEquatable`, and `IStructuralComparable`.

## Framework Compatibility (Collections + Extension Methods)

- All collections and extension methods should behave like their base/corelib counterparts for:
  - **Null handling**: ignore nulls or throw exactly as the framework would for the analogous API.
  - **Exception behavior**: throw the same exception types the framework throws for the analogous API. Prefer using the BCL-style exceptions (e.g., `ArgumentNullException`, `ArgumentOutOfRangeException`, `InvalidOperationException`) and avoid “custom” exception types for standard argument/state errors.
  - **Enumerator invalidation**: behaviors such as mutating a list during enumeration must throw `InvalidOperationException` (like `List<T>`), even when a naive array-style implementation would not.

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
