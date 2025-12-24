# Contributing

Thanks for helping improve Lod.RecordCollections! Keep contributions tightly scoped to the problem at hand, favor small focused diffs, and explain the intent behind every change. When in doubt, ask clarifying questions before refactoring, avoid drive-by cleanups, and prefer descriptive naming, consistent formatting, and exception-based error handling.

## Record Collection File Structure

- Implement every record collection (`RecordDictionary`, `RecordList`, `RecordSet`, and any new types) as a `partial` class split across focused files.
- Use `<Type>.cs` for constructors, core state, and invariants, `<Type>.Record.cs` for record contract overrides, and `<Type>.<Interface>.cs` for each interface implementation (for example `RecordSet.IEqualityComparer.cs`).
- When you add or remove supported interfaces, update the matching partial file instead of expanding the base class. This keeps diffs small and makes it obvious where a given behavior lives.
- Mirror the existing `RecordDictionary` layout when creating new collection types so that maintainers can navigate and review changes consistently.

## Comparers and Interface Implementations

- All equality, comparison, and interface implementations must **directly call the collection’s `Comparer`** (`IRecordCollectionComparer`) and must not implement additional in-class logic or route through other methods. This ensures predictable behavior when using custom comparers without requiring inheritance.
- This includes (but isn’t limited to) implementations of: `IEquatable<T>`, `IComparable<T>`, `IEqualityComparer`, `IEqualityComparer<T>`, `IComparer`, `IComparer<T>`, `IStructuralEquatable`, and `IStructuralComparable`.

## Framework Compatibility (Collections + Extension Methods)

- Collections and extension methods should match base/corelib behavior for the analogous API:
  - **Null handling**: ignore nulls or throw exactly as the framework would.
  - **Exception behavior**: throw the same exception types as the framework (e.g., `ArgumentNullException`, `ArgumentOutOfRangeException`, `InvalidOperationException`), and avoid introducing custom exception types for standard argument/state violations.
  - **Enumerator invalidation**: mutating a list during enumeration must throw `InvalidOperationException` (as `List<T>` does), even if a naive array-style implementation would not.
