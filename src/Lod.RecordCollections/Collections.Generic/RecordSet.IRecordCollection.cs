namespace System.Collections.Generic;

partial class RecordSet<T>
    : IRecordCollection<T>
{
    [DebuggerHidden]
    int IEqualityComparer<IRecordCollection<T>>.GetHashCode(IRecordCollection<T> obj) =>
        Comparer.GetHashCode(obj);

    [DebuggerHidden]
    bool IEquatable<IRecordCollection<T>>.Equals(IRecordCollection<T>? other) =>
        Comparer.Equals(this, other);

    [DebuggerHidden]
    bool IEqualityComparer<IRecordCollection<T>>.Equals(IRecordCollection<T>? x, IRecordCollection<T>? y) =>
        Comparer.Equals(x, y);
}
