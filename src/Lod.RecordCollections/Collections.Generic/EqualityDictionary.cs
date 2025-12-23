using System.Diagnostics;

namespace System.Collections.Generic;

/// <summary>
/// Represents a strongly typed dictionary of objects that can be accessed by index.
/// Provides methods to search, sort, and manipulate dictionarys.
/// Record dictionaries support value based comparison of dictionary data.
/// </summary>
/// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
/// <remarks>Uses an underlying collection of <see cref="Dictionary{TKey, TValue}"/>.</remarks>
public class EqualityDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IRecordCollection<KeyValuePair<TKey, TValue>>
    , IEnumerable, IEnumerable<KeyValuePair<TKey, TValue>>
    , ICollection, ICollection<KeyValuePair<TKey, TValue>>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    , IEquatable<EqualityDictionary<TKey, TValue>>, IEqualityComparer, IEqualityComparer<EqualityDictionary<TKey, TValue>>
    //, IComparable, IComparable<EqualityDictionary<TKey, TValue>>
    , IStructuralEquatable, IStructuralComparable
    where TKey : notnull
    where TValue : IEquatable<TValue>
{
    /// <summary>
    /// Gets the comparer used to compare elements and collections.
    /// </summary>
    public virtual new IRecordCollectionComparer Comparer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EqualityDictionary{TKey, TValue}"/> class that is empty and has the default initial capacity.
    /// </summary>
    public EqualityDictionary() : this(comparer: null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EqualityDictionary{TKey, TValue}"/> class that is empty,
    /// has the default initial capacity, and uses the specified comparer.
    /// </summary>
    /// <param name="comparer">The comparer used for record equality.</param>
    public EqualityDictionary(IRecordCollectionComparer? comparer) : base()
    {
        Comparer = comparer ?? RecordCollectionComparer.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EqualityDictionary{TKey, TValue}"/> class that uses the specified underlying dictionary.
    /// </summary>
    /// <param name="dictionary">An existing <see cref="EqualityDictionary{TKey, TValue}"/> to use as the underlying collection.</param>
    public EqualityDictionary(Dictionary<TKey, TValue> dictionary) : this(dictionary, comparer: null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EqualityDictionary{TKey, TValue}"/> class that uses the specified underlying dictionary
    /// and comparer.
    /// </summary>
    /// <param name="dictionary">An existing <see cref="EqualityDictionary{TKey, TValue}"/> to use as the underlying collection.</param>
    /// <param name="comparer">The comparer used for record equality.</param>
    public EqualityDictionary(Dictionary<TKey, TValue> dictionary, IRecordCollectionComparer? comparer) : base(dictionary)
    {
        Comparer = comparer ?? RecordCollectionComparer.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EqualityDictionary{TKey, TValue}"/> class that
    /// contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new dictionary.</param>
    public EqualityDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : this(collection, comparer: null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EqualityDictionary{TKey, TValue}"/> class that
    /// contains elements copied from the specified collection and uses the specified comparer.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new dictionary.</param>
    /// <param name="comparer">The comparer used for record equality.</param>
    public EqualityDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IRecordCollectionComparer? comparer)
        : base(collection?.ToDictionary(kv => kv.Key, kv => kv.Value)!)
    {
        Comparer = comparer ?? RecordCollectionComparer.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EqualityDictionary{TKey, TValue}"/> class that is empty and has the specified initial capacity.
    /// </summary>
    /// <param name="capacity">The number of elements that the new dictionary can initially store.</param>
    public EqualityDictionary(int capacity) : this(capacity, comparer: null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EqualityDictionary{TKey, TValue}"/> class that is empty, has the specified initial capacity,
    /// and uses the specified comparer.
    /// </summary>
    /// <param name="capacity">The number of elements that the new dictionary can initially store.</param>
    /// <param name="comparer">The comparer used for record equality.</param>
    public EqualityDictionary(int capacity, IRecordCollectionComparer? comparer) : base(new Dictionary<TKey, TValue>(capacity))
    {
        Comparer = comparer ?? RecordCollectionComparer.Default;
    }

    #region Record-like Specification

    /// <summary>
    /// Initializes a new instance of the <see cref="EqualityDictionary{TKey, TValue}"/> class that uses records from an existing collection.
    /// </summary>
    /// <param name="original">An existing <see cref="EqualityDictionary{TKey, TValue}"/> to clone into the new record.</param>
    protected EqualityDictionary(EqualityDictionary<TKey, TValue> original)
        : base(original.Select(o => new KeyValuePair<TKey, TValue>(o.Key, RecordCloner.TryClone(o.Value)!)).ToDictionary(kv => kv.Key, kv => kv.Value))
    {
        Comparer = original?.Comparer ?? RecordCollectionComparer.Default;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => Comparer.GetHashCode(this);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Comparer.Equals(this, obj);

    /// <summary>
    /// Returns a value indicating whether the collection is equal to another <see cref="Dictionary{TKey, TValue}"/>.
    /// </summary>
    public bool Equals(Dictionary<TKey, TValue> other) => Comparer.Equals(this, other);

    /// <summary>
    /// Returns a value indicating whether the collection is equal to another <see cref="EqualityDictionary{TKey, TValue}"/>.
    /// </summary>
    public virtual bool Equals(EqualityDictionary<TKey, TValue>? other) => Comparer.Equals(this, other);

    /// <summary>
    /// Returns a value indicating whether two <see cref="EqualityDictionary{TKey, TValue}"/> represent the same collection of records.
    /// </summary>
    public static bool operator ==(EqualityDictionary<TKey, TValue> left, EqualityDictionary<TKey, TValue> right) =>
        left.Equals(right);

    /// <summary>
    /// Returns a value indicating whether two <see cref="EqualityDictionary{TKey, TValue}"/> represent a different collection of records.
    /// </summary>
    public static bool operator !=(EqualityDictionary<TKey, TValue> left, EqualityDictionary<TKey, TValue> right) =>
        !left.Equals(right);

    #endregion

    #region IEqualityComparer

    /// <summary>
    /// Determines whether the specified objects are equal.
    /// </summary>
    public bool Equals(EqualityDictionary<TKey, TValue>? x, EqualityDictionary<TKey, TValue>? y) =>
        Comparer.Equals(x, y);

    [DebuggerHidden]
    bool IEqualityComparer.Equals(object? x, object? y) =>
        x is EqualityDictionary<TKey, TValue> set && Comparer.Equals(set, y);

    /// <summary>
    /// Returns a hash code for the specified object.
    /// </summary>
    public int GetHashCode(EqualityDictionary<TKey, TValue> x) =>
        Comparer.GetHashCode(x);

    [DebuggerHidden]
    int IEqualityComparer.GetHashCode(object obj) =>
        obj is EqualityDictionary<TKey, TValue> set ? Comparer.GetHashCode(set) : 0;

    #endregion

    #region IStructuralEquatable

    [DebuggerHidden]
    bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer) =>
        comparer.Equals(this, other);

    [DebuggerHidden]
    int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) =>
        comparer.GetHashCode(this);

    #endregion

    #region IComparable

    //[DebuggerHidden]
    //int IComparable.CompareTo(object obj) => obj is EqualityDictionary<TKey, TValue> set ? CompareTo(set) : -1;

    //public int CompareTo(EqualityDictionary<TKey, TValue> other) =>

    #endregion

    #region IStructuralComparable

    [DebuggerHidden]
    int IStructuralComparable.CompareTo(object? other, IComparer comparer) =>
        comparer.Compare(this, other);

    #endregion

    #region IRecordCollection

    /// <summary>
    /// Returns a value indicating whether an <paramref name="other"/> collection is equal to the current instance.
    /// </summary>
    /// <param name="other">The collection to compare the current collection to.</param>
    /// <return>True if the underlying collection's elements are equivalent to the current collection.</return>
    public bool Equals(IReadOnlyRecordCollection? other) =>
        Comparer.Equals(this, other);

    /// <summary>
    /// Returns a value indicating whether the <paramref name="left"/> collection is equal to the <paramref name="right"/> collection.
    /// </summary>
    /// <param name="left">The original collection to compare the other collection to.</param>
    /// <param name="right">The collection to compare the current collection to.</param>
    /// <return>True if the underlying collection's elements are equivalent to the current collection.</return>
    public bool Equals(IReadOnlyRecordCollection? left, IReadOnlyRecordCollection? right) =>
        Comparer.Equals(left, right);

    [DebuggerHidden]
    bool IEquatable<IReadOnlyRecordCollection<KeyValuePair<TKey, TValue>>>.Equals(IReadOnlyRecordCollection<KeyValuePair<TKey, TValue>>? other) =>
        Comparer.Equals(this, other);

    [DebuggerHidden]
    bool IEqualityComparer<IReadOnlyRecordCollection<KeyValuePair<TKey, TValue>>>.Equals(IReadOnlyRecordCollection<KeyValuePair<TKey, TValue>>? x, IReadOnlyRecordCollection<KeyValuePair<TKey, TValue>>? y) =>
        Comparer.Equals(x, y);

    [DebuggerHidden]
    int IEqualityComparer<IRecordCollection<KeyValuePair<TKey, TValue>>>.GetHashCode(IRecordCollection<KeyValuePair<TKey, TValue>> obj) =>
        Comparer.GetHashCode(obj);

    [DebuggerHidden]
    bool IEquatable<IRecordCollection<KeyValuePair<TKey, TValue>>>.Equals(IRecordCollection<KeyValuePair<TKey, TValue>>? other) =>
        Comparer.Equals(this, other);

    [DebuggerHidden]
    bool IEqualityComparer<IRecordCollection<KeyValuePair<TKey, TValue>>>.Equals(IRecordCollection<KeyValuePair<TKey, TValue>>? x, IRecordCollection<KeyValuePair<TKey, TValue>>? y) =>
        Comparer.Equals(x, y);

    [DebuggerHidden]
    int IEqualityComparer<IReadOnlyRecordCollection<KeyValuePair<TKey, TValue>>>.GetHashCode(IReadOnlyRecordCollection<KeyValuePair<TKey, TValue>> obj) =>
        Comparer.GetHashCode(obj);

    #endregion
}
