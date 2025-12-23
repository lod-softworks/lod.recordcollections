namespace System.Collections.Generic;

partial class RecordDictionary<TKey, TValue>
    : IStructuralComparable
{
    [DebuggerHidden]
    int IStructuralComparable.CompareTo(object? other, IComparer comparer) =>
        comparer.Compare(this, other);
}
