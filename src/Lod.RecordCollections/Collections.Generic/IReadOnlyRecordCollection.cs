namespace System.Collections.Generic;

/// <summary>
/// A read-only collection of strongly typed, record-like values.
/// Record collections support value based comparison.
/// </summary>
/// <typeparam name="T">The type of the elements in the colllection.</typeparam>
public interface IReadOnlyRecordCollection<T>
    : IReadOnlyRecordCollection
    , IReadOnlyCollection<T>
    //, IComparable<ICollection<T>>
    , IEquatable<IReadOnlyRecordCollection<T>>
    , IEqualityComparer<IReadOnlyRecordCollection<T>>
{ }
