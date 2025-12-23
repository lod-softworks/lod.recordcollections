namespace System.Collections.Generic;

partial class RecordDictionary<TKey, TValue>
    : IEqualityComparer, IEqualityComparer<RecordDictionary<TKey, TValue>>
    , IEqualityComparer<IReadOnlyRecordCollection>
    , IEqualityComparer<IReadOnlyRecordCollection<KeyValuePair<TKey, TValue>>>
    , IEqualityComparer<IRecordCollection<KeyValuePair<TKey, TValue>>>
{
    [DebuggerHidden]
    bool IEqualityComparer.Equals(object? x, object? y) =>
        x is IReadOnlyRecordCollection xRecordCollection && Comparer.Equals(xRecordCollection, y);

    [DebuggerHidden]
    bool IEqualityComparer<RecordDictionary<TKey, TValue>>.Equals(RecordDictionary<TKey, TValue>? x, RecordDictionary<TKey, TValue>? y) =>
        Comparer.Equals(x, y);

    [DebuggerHidden]
    bool IEqualityComparer<IReadOnlyRecordCollection<KeyValuePair<TKey, TValue>>>.Equals(IReadOnlyRecordCollection<KeyValuePair<TKey, TValue>>? x, IReadOnlyRecordCollection<KeyValuePair<TKey, TValue>>? y) =>
        Comparer.Equals(x, y);

    [DebuggerHidden]
    bool IEqualityComparer<IReadOnlyRecordCollection>.Equals(IReadOnlyRecordCollection? x, IReadOnlyRecordCollection? y) =>
        Comparer.Equals(x, y);

    [DebuggerHidden]
    bool IEqualityComparer<IRecordCollection<KeyValuePair<TKey, TValue>>>.Equals(IRecordCollection<KeyValuePair<TKey, TValue>>? x, IRecordCollection<KeyValuePair<TKey, TValue>>? y) =>
        Comparer.Equals(x, y);

    [DebuggerHidden]
    int IEqualityComparer<RecordDictionary<TKey, TValue>>.GetHashCode(RecordDictionary<TKey, TValue> x) =>
        Comparer.GetHashCode(x);

    [DebuggerHidden]
    int IEqualityComparer.GetHashCode(object obj) =>
        obj is IReadOnlyRecordCollection recordCollection ? Comparer.GetHashCode(recordCollection) : default;

    [DebuggerHidden]
    int IEqualityComparer<IReadOnlyRecordCollection>.GetHashCode(IReadOnlyRecordCollection obj) =>
        Comparer.GetHashCode(obj);

    [DebuggerHidden]
    int IEqualityComparer<IReadOnlyRecordCollection<KeyValuePair<TKey, TValue>>>.GetHashCode(IReadOnlyRecordCollection<KeyValuePair<TKey, TValue>> obj) =>
        Comparer.GetHashCode(obj);

    [DebuggerHidden]
    int IEqualityComparer<IRecordCollection<KeyValuePair<TKey, TValue>>>.GetHashCode(IRecordCollection<KeyValuePair<TKey, TValue>> obj) =>
        Comparer.GetHashCode(obj);
}
