namespace System.Collections.Generic;

partial class RecordSet<T>
    : ICollection
    , ICollection<T>
{
    [DebuggerHidden]
    bool ICollection.IsSynchronized => false;

    [DebuggerHidden]
    object ICollection.SyncRoot => this;

    [DebuggerHidden]
    void ICollection.CopyTo(Array array, int index)
    {
        foreach (T item in this)
        {
            array.SetValue(item, index++);
        }
    }
}
