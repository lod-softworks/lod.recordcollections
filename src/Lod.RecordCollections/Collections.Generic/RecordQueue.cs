namespace System.Collections.Generic;

/// <summary>
/// Represents a first-in, first-out (FIFO) collection of objects.
/// Record queues support value based comparison.
/// </summary>
/// <typeparam name="T">The type of elements in the queue.</typeparam>
public partial class RecordQueue<T> : Queue<T>
    where T : IEquatable<T>
{
    /// <summary>
    /// Gets the comparer used to compare elements and collections.
    /// </summary>
    public virtual IRecordCollectionComparer Comparer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordQueue{T}"/> class that is empty and has the default initial capacity.
    /// </summary>
    public RecordQueue()
        : this(comparer: null)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordQueue{T}"/> class that is empty,
    /// has the default initial capacity, and uses the specified comparer.
    /// </summary>
    /// <param name="comparer">The comparer used for record equality.</param>
    public RecordQueue(IRecordCollectionComparer? comparer)
        : base()
    {
        Comparer = comparer ?? RecordCollectionComparer.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordQueue{T}"/> class that
    /// contains elements copied from the specified collection and has sufficient capacity
    /// to accommodate the number of elements copied.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new queue.</param>
    public RecordQueue(IEnumerable<T> collection)
        : this(collection, comparer: null)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordQueue{T}"/> class that
    /// contains elements copied from the specified collection and uses the specified comparer.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new queue.</param>
    /// <param name="comparer">The comparer used for record equality.</param>
    public RecordQueue(IEnumerable<T> collection, IRecordCollectionComparer? comparer)
        : base(collection)
    {
        Comparer = comparer ?? RecordCollectionComparer.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordQueue{T}"/> class that is empty and has the specified initial capacity.
    /// </summary>
    /// <param name="capacity">The number of elements that the new queue can initially store.</param>
    public RecordQueue(int capacity)
        : this(capacity, comparer: null)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordQueue{T}"/> class that is empty, has the specified initial capacity,
    /// and uses the specified comparer.
    /// </summary>
    /// <param name="capacity">The number of elements that the new queue can initially store.</param>
    /// <param name="comparer">The comparer used for record equality.</param>
    public RecordQueue(int capacity, IRecordCollectionComparer? comparer)
        : base(capacity)
    {
        Comparer = comparer ?? RecordCollectionComparer.Default;
    }
}
