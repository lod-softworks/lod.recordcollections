namespace System.Collections.Generic;

partial class RecordSet<T>
    : IEqualityComparer
    , IEqualityComparer<RecordSet<T>>
{
    /// <summary>
    /// Determines whether the specified objects are equal.
    /// </summary>
    public bool Equals(RecordSet<T>? x, RecordSet<T>? y) =>
        Comparer.Equals(x, y);

    [DebuggerHidden]
    bool IEqualityComparer.Equals(object? x, object? y) =>
        x is RecordSet<T> set && Comparer.Equals(set, y);

    /// <summary>
    /// Returns a hash code for the specified object.
    /// </summary>
    public int GetHashCode(RecordSet<T> x) =>
        Comparer.GetHashCode(x);

    [DebuggerHidden]
    int IEqualityComparer.GetHashCode(object? obj) =>
        obj is RecordSet<T> set ? Comparer.GetHashCode(set) : 0;
}
