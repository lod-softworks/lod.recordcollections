namespace System.Collections.Generic;

partial class RecordSet<T>
    : IReadOnlyRecordCollection
    , IReadOnlyRecordCollection<T>
{
    /// <summary>
    /// Returns a value indicating whether an <paramref name="other"/> collection is equal to the current instance.
    /// </summary>
    /// <param name="other">The collection to compare the current collection to.</param>
    /// <return>True if the underlying collection's elements are equivalent to the current collection.</return>
    public bool Equals(IReadOnlyRecordCollection? other) =>
        Comparer.Equals(this, other);

    /// <summary>
    /// Returns a value indicating whether the <paramref name="left"/> collection is equal to the <paramref name="right"/> collection.
    /// </summary>
    /// <param name="left">The original collection to compare the other collection to.</param>
    /// <param name="right">The collection to compare the current collection to.</param>
    /// <return>True if the underlying collection's elements are equivalent to the current collection.</return>
    public bool Equals(IReadOnlyRecordCollection? left, IReadOnlyRecordCollection? right) =>
        Comparer.Equals(left, right);

    [DebuggerHidden]
    bool IEquatable<IReadOnlyRecordCollection<T>>.Equals(IReadOnlyRecordCollection<T>? other) =>
        Comparer.Equals(this, other);

    [DebuggerHidden]
    bool IEqualityComparer<IReadOnlyRecordCollection<T>>.Equals(IReadOnlyRecordCollection<T>? x, IReadOnlyRecordCollection<T>? y) =>
        Comparer.Equals(x, y);

    [DebuggerHidden]
    int IEqualityComparer<IReadOnlyRecordCollection<T>>.GetHashCode(IReadOnlyRecordCollection<T> obj) =>
        Comparer.GetHashCode(obj);
}
