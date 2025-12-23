namespace System.Collections.Generic;

/// <summary>
/// Represents a strongly typed list of objects that can be accessed by index.
/// Provides methods to search, sort, and manipulate lists.
/// Record lists support value based comparison.
/// </summary>
/// <typeparam name="T">The type of elements in the list.</typeparam>
public partial class RecordList<T> : List<T>
    where T : IEquatable<T>
{
    /// <summary>
    /// Gets the comparer used to compare elements and collections.
    /// </summary>
    public virtual IRecordCollectionComparer Comparer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordList{T}"/> class that is empty and has the default initial capacity.
    /// </summary>
    public RecordList()
        : this(comparer: null)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordList{T}"/> class that is empty,
    /// has the default initial capacity, and uses the specified comparer.
    /// </summary>
    /// <param name="comparer">The comparer used for record equality.</param>
    public RecordList(IRecordCollectionComparer? comparer)
        : base()
    {
        Comparer = comparer ?? RecordCollectionComparer.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordList{T}"/> class that uses the specified underlying list.
    /// </summary>
    /// <param name="list">An existing <see cref="List{T}"/> to use as the underlying collection.</param>
    public RecordList(List<T> list)
        : this(list, comparer: null)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordList{T}"/> class that uses the specified underlying list
    /// and comparer.
    /// </summary>
    /// <param name="list">An existing <see cref="List{T}"/> to use as the underlying collection.</param>
    /// <param name="comparer">The comparer used for record equality.</param>
    public RecordList(List<T> list, IRecordCollectionComparer? comparer)
        : base(list)
    {
        Comparer = comparer ?? RecordCollectionComparer.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordList{T}"/> class that
    /// contains elements copied from the specified collection and has sufficient capacity
    /// to accommodate the number of elements copied.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new list.</param>
    public RecordList(IEnumerable<T> collection)
        : this(collection, comparer: null)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordList{T}"/> class that
    /// contains elements copied from the specified collection, has sufficient capacity
    /// to accommodate the number of elements copied, and uses the specified comparer.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new list.</param>
    /// <param name="comparer">The comparer used for record equality.</param>
    public RecordList(IEnumerable<T> collection, IRecordCollectionComparer? comparer)
        : base(collection)
    {
        Comparer = comparer ?? RecordCollectionComparer.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordList{T}"/> class that is empty and has the specified initial capacity.
    /// </summary>
    /// <param name="capacity">The number of elements that the new list can initially store.</param>
    public RecordList(int capacity)
        : this(capacity, comparer: null)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordList{T}"/> class that is empty, has the specified initial capacity,
    /// and uses the specified comparer.
    /// </summary>
    /// <param name="capacity">The number of elements that the new list can initially store.</param>
    /// <param name="comparer">The comparer used for record equality.</param>
    public RecordList(int capacity, IRecordCollectionComparer? comparer)
        : base(capacity)
    {
        Comparer = comparer ?? RecordCollectionComparer.Default;
    }
}
