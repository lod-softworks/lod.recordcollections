namespace System.Collections.Generic;

partial class RecordDictionary<TKey, TValue>
    : IEquatable<RecordDictionary<TKey, TValue>>
    , IEquatable<IReadOnlyRecordCollection>
    , IEquatable<IReadOnlyRecordCollection<KeyValuePair<TKey, TValue>>>
    , IEquatable<IRecordCollection<KeyValuePair<TKey, TValue>>>
{
    [DebuggerHidden]
    bool IEquatable<IReadOnlyRecordCollection>.Equals(IReadOnlyRecordCollection? other) =>
        Comparer.Equals(this, other);

    [DebuggerHidden]
    bool IEquatable<IRecordCollection<KeyValuePair<TKey, TValue>>>.Equals(IRecordCollection<KeyValuePair<TKey, TValue>>? other) =>
        Comparer.Equals(this, other);

    [DebuggerHidden]
    bool IEquatable<IReadOnlyRecordCollection<KeyValuePair<TKey, TValue>>>.Equals(IReadOnlyRecordCollection<KeyValuePair<TKey, TValue>>? other) =>
        Comparer.Equals(this, other);
}
