namespace System.Collections.Generic;

partial class RecordSet<T>
    : IStructuralEquatable
{
    [DebuggerHidden]
    bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer) =>
        comparer.Equals(this, other);

    [DebuggerHidden]
    int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) =>
        comparer.GetHashCode(this);
}
