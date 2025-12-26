# Contributing

Thanks for helping improve Lod.RecordCollections! Keep contributions tightly scoped to the problem at hand, favor small focused diffs, and explain the intent behind every change. When in doubt, ask clarifying questions before refactoring, avoid drive-by cleanups, and prefer descriptive naming, consistent formatting, and exception-based error handling.

## Record Collection File Structure

- Implement every record collection (`RecordDictionary`, `RecordList`, `RecordSet`, and any new types) as a `partial` class split across focused files.
- Use `<Type>.cs` for constructors, core state, and invariants, `<Type>.Record.cs` for record contract overrides, and `<Type>.<Interface>.cs` for each interface implementation (for example `RecordSet.IEqualityComparer.cs`).
- When you add or remove supported interfaces, update the matching partial file instead of expanding the base class. This keeps diffs small and makes it obvious where a given behavior lives.
- Mirror the existing `RecordDictionary` layout when creating new collection types so that maintainers can navigate and review changes consistently.

## Achieving Record State

Record types in .NET do not have any specific type flags or identifiers, the language identifies them based on the existence of certain class members that fulfill the `record` promise. 

Record Collections achieve `record` compatibility and semantics through a combination of inheritance, manual method overrides, and post-compilation IL modification:

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

- All equality, comparison, and interface implementations must **directly call the collection’s `Comparer`** (`IRecordCollectionComparer`) and must not implement additional in-class logic or route through other methods. This ensures predictable behavior when using custom comparers without requiring inheritance.
- This includes (but isn’t limited to) implementations of: `IEquatable<T>`, `IComparable<T>`, `IEqualityComparer`, `IEqualityComparer<T>`, `IComparer`, `IComparer<T>`, `IStructuralEquatable`, and `IStructuralComparable`.

## Framework Compatibility (Collections + Extension Methods)

- Collections and extension methods should match base/corelib behavior for the analogous API:
  - **Null handling**: ignore nulls or throw exactly as the framework would.
  - **Exception behavior**: throw the same exception types as the framework (e.g., `ArgumentNullException`, `ArgumentOutOfRangeException`, `InvalidOperationException`), and avoid introducing custom exception types for standard argument/state violations.
  - **Enumerator invalidation**: mutating a list during enumeration must throw `InvalidOperationException` (as `List<T>` does), even if a naive array-style implementation would not.
