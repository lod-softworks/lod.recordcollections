# Contributing

Thanks for helping improve Lod.RecordCollections! Keep contributions tightly scoped to the problem at hand, favor small focused diffs, and explain the intent behind every change. When in doubt, ask clarifying questions before refactoring, avoid drive-by cleanups, and prefer descriptive naming, consistent formatting, and exception-based error handling.

## Record Collection File Structure

- Implement every record collection (`RecordDictionary`, `RecordList`, `RecordSet`, and any new types) as a `partial` class split across focused files.
- Use `<Type>.cs` for constructors, core state, and invariants, `<Type>.Record.cs` for record contract overrides, and `<Type>.<Interface>.cs` for each interface implementation (for example `RecordSet.IEqualityComparer.cs`).
- When you add or remove supported interfaces, update the matching partial file instead of expanding the base class. This keeps diffs small and makes it obvious where a given behavior lives.
- Mirror the existing `RecordDictionary` layout when creating new collection types so that maintainers can navigate and review changes consistently.
