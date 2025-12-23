namespace System.Collections.Generic;

/// <summary>
/// Represents a strongly typed set of objects that can be accessed by index.
/// Provides methods to search, sort, and manipulate sets.
/// Record sets support value based comparison.
/// </summary>
/// <typeparam name="T">The type of elements in the set.</typeparam>
public partial class RecordSet<T> : HashSet<T>
    where T : IEquatable<T>
{
    /// <summary>
    /// Gets the comparer used to compare elements and collections.
    /// </summary>
    public virtual new IRecordCollectionComparer Comparer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordSet{T}"/> class that is empty and has the default initial capacity.
    /// </summary>
    public RecordSet()
        : this(comparer: null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordSet{T}"/> class that is empty,
    /// has the default initial capacity, and uses the specified comparer.
    /// </summary>
    /// <param name="comparer">The comparer used for record equality.</param>
    public RecordSet(IRecordCollectionComparer? comparer)
        : base()
    {
        Comparer = comparer ?? RecordCollectionComparer.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordSet{T}"/> class that uses the specified underlying set.
    /// </summary>
    /// <param name="hashSet">An existing <see cref="HashSet{T}"/> to use as the underlying collection.</param>
    public RecordSet(HashSet<T> hashSet)
        : this(hashSet, comparer: null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordSet{T}"/> class that uses the specified underlying set
    /// and comparer.
    /// </summary>
    /// <param name="hashSet">An existing <see cref="HashSet{T}"/> to use as the underlying collection.</param>
    /// <param name="comparer">The comparer used for record equality.</param>
    public RecordSet(HashSet<T> hashSet, IRecordCollectionComparer? comparer)
        : base(hashSet)
    {
        Comparer = comparer ?? RecordCollectionComparer.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordSet{T}"/> class that
    /// contains elements copied from the specified collection and has sufficient capacity
    /// to accommodate the number of elements copied.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new set.</param>
    public RecordSet(IEnumerable<T> collection)
        : this(collection, comparer: null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordSet{T}"/> class that
    /// contains elements copied from the specified collection and uses the specified comparer.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new set.</param>
    /// <param name="comparer">The comparer used for record equality.</param>
    public RecordSet(IEnumerable<T> collection, IRecordCollectionComparer? comparer)
        : base(new HashSet<T>(collection))
    {
        Comparer = comparer ?? RecordCollectionComparer.Default;
    }

#if NET48_OR_GREATER || NET6_0_OR_GREATER

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordSet{T}"/> class that is empty and has the specified initial capacity.
    /// </summary>
    /// <param name="capacity">The number of elements that the new set can initially store.</param>
    public RecordSet(int capacity)
        : this(capacity, comparer: null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordSet{T}"/> class that is empty, has the specified initial capacity,
    /// and uses the specified comparer.
    /// </summary>
    /// <param name="capacity">The number of elements that the new set can initially store.</param>
    /// <param name="comparer">The comparer used for record equality.</param>
    public RecordSet(int capacity, IRecordCollectionComparer? comparer)
        : base(new HashSet<T>(capacity))
    {
        Comparer = comparer ?? RecordCollectionComparer.Default;
    }

#endif
}
