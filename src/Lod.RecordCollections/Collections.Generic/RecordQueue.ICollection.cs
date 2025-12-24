namespace System.Collections.Generic;

partial class RecordQueue<T>
    : ICollection
    , ICollection<T>
{
    [DebuggerHidden]
    void ICollection<T>.Add(T item) =>
        Enqueue(item);

    [DebuggerHidden]
    bool ICollection<T>.Remove(T item) =>
        throw new NotSupportedException("Queue does not support remove.");

    [DebuggerHidden]
    bool ICollection<T>.IsReadOnly => false;
}
