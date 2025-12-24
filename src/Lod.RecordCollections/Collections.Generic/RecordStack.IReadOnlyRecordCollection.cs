namespace System.Collections.Generic;

partial class RecordStack<T>
    : IReadOnlyRecordCollection
    , IReadOnlyRecordCollection<T>
{
#if !NET6_0_OR_GREATER
    [DebuggerHidden]
    bool IReadOnlyRecordCollection.Equals(IReadOnlyRecordCollection? other) =>
        Comparer.Equals(this, other);

    [DebuggerHidden]
    bool IReadOnlyRecordCollection.Equals(IReadOnlyRecordCollection? left, IReadOnlyRecordCollection? right) =>
        Comparer.Equals(left, right);
#endif
}
