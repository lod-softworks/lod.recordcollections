namespace System.Collections.Generic;

partial class RecordStack<T>
    : ICollection
    , ICollection<T>
{
    [DebuggerHidden]
    void ICollection<T>.Add(T item) =>
        Push(item);

    [DebuggerHidden]
    bool ICollection<T>.Remove(T item) =>
        throw new NotSupportedException("Stack does not support remove.");

    [DebuggerHidden]
    bool ICollection<T>.IsReadOnly => false;
}
