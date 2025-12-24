namespace System.Collections.Generic;

partial class RecordQueue<T>
    : IStructuralComparable
{
    [DebuggerHidden]
    int IStructuralComparable.CompareTo(object? other, IComparer comparer) =>
        comparer.Compare(this, other);
}
