namespace System.Collections.Generic;

partial class RecordStack<T>
    : IEquatable<RecordStack<T>>
    , IEquatable<IReadOnlyRecordCollection>
    , IEquatable<IReadOnlyRecordCollection<T>>
    , IEquatable<IRecordCollection<T>>
{
    [DebuggerHidden]
    bool IEquatable<IReadOnlyRecordCollection>.Equals(IReadOnlyRecordCollection? other) =>
        Comparer.Equals(this, other);

    [DebuggerHidden]
    bool IEquatable<IRecordCollection<T>>.Equals(IRecordCollection<T>? other) =>
        Comparer.Equals(this, other);

    [DebuggerHidden]
    bool IEquatable<IReadOnlyRecordCollection<T>>.Equals(IReadOnlyRecordCollection<T>? other) =>
        Comparer.Equals(this, other);
}
