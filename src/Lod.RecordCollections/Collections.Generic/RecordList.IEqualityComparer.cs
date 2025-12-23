namespace System.Collections.Generic;

partial class RecordList<T>
    : IEqualityComparer
    , IEqualityComparer<RecordList<T>>
{
    /// <summary>
    /// Determines whether the specified objects are equal.
    /// </summary>
    public bool Equals(RecordList<T>? x, RecordList<T>? y) =>
        Comparer.Equals(x, y);

    [DebuggerHidden]
    bool IEqualityComparer.Equals(object? x, object? y) =>
        x is RecordList<T> list && Comparer.Equals(list, y);

    /// <summary>
    /// Returns a hash code for the specified object.
    /// </summary>
    public int GetHashCode(RecordList<T> x) =>
        Comparer.GetHashCode(x);

    [DebuggerHidden]
    int IEqualityComparer.GetHashCode(object obj) =>
        obj is RecordList<T> list ? Comparer.GetHashCode(list) : 0;
}
