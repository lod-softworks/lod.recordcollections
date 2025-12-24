namespace System.Collections.Generic;

partial class RecordStack<T>
    : IEqualityComparer
    , IEqualityComparer<RecordStack<T>>
    , IEqualityComparer<IReadOnlyRecordCollection>
    , IEqualityComparer<IReadOnlyRecordCollection<T>>
    , IEqualityComparer<IRecordCollection<T>>
{
    [DebuggerHidden]
    bool IEqualityComparer.Equals(object? x, object? y) =>
        x is IReadOnlyRecordCollection xRecordCollection && Comparer.Equals(xRecordCollection, y);

    [DebuggerHidden]
    bool IEqualityComparer<RecordStack<T>>.Equals(RecordStack<T>? x, RecordStack<T>? y) =>
        Comparer.Equals(x, y);

    [DebuggerHidden]
    bool IEqualityComparer<IReadOnlyRecordCollection>.Equals(IReadOnlyRecordCollection? x, IReadOnlyRecordCollection? y) =>
        Comparer.Equals(x, y);

    [DebuggerHidden]
    bool IEqualityComparer<IReadOnlyRecordCollection<T>>.Equals(IReadOnlyRecordCollection<T>? x, IReadOnlyRecordCollection<T>? y) =>
        Comparer.Equals(x, y);

    [DebuggerHidden]
    bool IEqualityComparer<IRecordCollection<T>>.Equals(IRecordCollection<T>? x, IRecordCollection<T>? y) =>
        Comparer.Equals(x, y);

    [DebuggerHidden]
    int IEqualityComparer.GetHashCode(object? obj) =>
        obj is IReadOnlyRecordCollection recordCollection ? Comparer.GetHashCode(recordCollection) : default;

    [DebuggerHidden]
    int IEqualityComparer<RecordStack<T>>.GetHashCode(RecordStack<T> obj) =>
        Comparer.GetHashCode(obj);

    [DebuggerHidden]
    int IEqualityComparer<IReadOnlyRecordCollection>.GetHashCode(IReadOnlyRecordCollection obj) =>
        Comparer.GetHashCode(obj);

    [DebuggerHidden]
    int IEqualityComparer<IReadOnlyRecordCollection<T>>.GetHashCode(IReadOnlyRecordCollection<T> obj) =>
        Comparer.GetHashCode(obj);

    [DebuggerHidden]
    int IEqualityComparer<IRecordCollection<T>>.GetHashCode(IRecordCollection<T> obj) =>
        Comparer.GetHashCode(obj);
}
