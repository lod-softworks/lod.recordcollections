using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Collections.Generic;

/// <summary>
/// Represents a strongly typed set of objects that can be accessed by index.
/// Provides methods to search, sort, and manipulate sets.
/// Record sets support value based comparison.
/// </summary>
/// <typeparam name="T">The type of elements in the set.</typeparam>
public class RecordSet<T> : HashSet<T>, IRecordCollection<T>
    , IEnumerable, IEnumerable<T>
    , ICollection, ICollection<T>, IReadOnlyCollection<T>
    , IEquatable<RecordSet<T>>, IEqualityComparer, IEqualityComparer<RecordSet<T>>
    //, IComparable, IComparable<RecordSet<T>>
    , IStructuralEquatable, IStructuralComparable
    where T : IEquatable<T>
{
    /// <summary>
    /// Gets the comparer used to compare elements and collections.
    /// </summary>
    public virtual new IRecordCollectionComparer Comparer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordSet{T}"/> class that is empty and has the default initial capacity.
    /// </summary>
    public RecordSet() : this(comparer: null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordSet{T}"/> class that is empty,
    /// has the default initial capacity, and uses the specified comparer.
    /// </summary>
    /// <param name="comparer">The comparer used for record equality.</param>
    public RecordSet(IRecordCollectionComparer? comparer) : base()
    {
        Comparer = comparer ?? RecordCollectionComparer.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordSet{T}"/> class that uses the specified underlying set.
    /// </summary>
    /// <param name="hashSet">An existing <see cref="HashSet{T}"/> to use as the underlying collection.</param>
    public RecordSet(HashSet<T> hashSet) : this(hashSet, comparer: null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordSet{T}"/> class that uses the specified underlying set
    /// and comparer.
    /// </summary>
    /// <param name="hashSet">An existing <see cref="HashSet{T}"/> to use as the underlying collection.</param>
    /// <param name="comparer">The comparer used for record equality.</param>
    public RecordSet(HashSet<T> hashSet, IRecordCollectionComparer? comparer) : base(hashSet)
    {
        Comparer = comparer ?? RecordCollectionComparer.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordSet{T}"/> class that
    /// contains elements copied from the specified collection and has sufficient capacity
    /// to accommodate the number of elements copied.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new set.</param>
    public RecordSet(IEnumerable<T> collection) : this(collection, comparer: null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordSet{T}"/> class that
    /// contains elements copied from the specified collection and uses the specified comparer.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new set.</param>
    /// <param name="comparer">The comparer used for record equality.</param>
    public RecordSet(IEnumerable<T> collection, IRecordCollectionComparer? comparer) : base(new HashSet<T>(collection))
    {
        Comparer = comparer ?? RecordCollectionComparer.Default;
    }

#if NET48_OR_GREATER || NET6_0_OR_GREATER

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordSet{T}"/> class that is empty and has the specified initial capacity.
    /// </summary>
    /// <param name="capacity">The number of elements that the new set can initially store.</param>
    public RecordSet(int capacity) : this(capacity, comparer: null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordSet{T}"/> class that is empty, has the specified initial capacity,
    /// and uses the specified comparer.
    /// </summary>
    /// <param name="capacity">The number of elements that the new set can initially store.</param>
    /// <param name="comparer">The comparer used for record equality.</param>
    public RecordSet(int capacity, IRecordCollectionComparer? comparer) : base(new HashSet<T>(capacity))
    {
        Comparer = comparer ?? RecordCollectionComparer.Default;
    }

#endif

    #region Record Specification

    /// <summary>
    /// Gets the record equality contract for this collection.
    /// </summary>
    // [RecordImp!]: This needs to be protected, virtual, returning it's own type to meet the `record` spec.
    protected virtual Type EqualityContract => typeof(RecordSet<T>);

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordSet{T}"/> class that uses records from an existing collection.
    /// </summary>
    /// <param name="original">An existing <see cref="RecordSet{T}"/> to clone into the new record.</param>
    // [RecordImp!]: This needs to be protected, non-null with no null checks to meet the `record` spec.
    protected RecordSet(RecordSet<T> original) : base(original.Select(o => RecordCloner.TryClone(o)!))
    {
        Comparer = original?.Comparer ?? RecordCollectionComparer.Default;
    }

    /// <inheritdoc/>
    // [RecordImp!]: This needs to be overriden to meet the `record` spec.
    public override int GetHashCode() => Comparer.GetHashCode(this);

    /// <inheritdoc/>
    // [RecordImp!]: This needs to be overriden to meet the `record` spec.
    public override bool Equals(object? obj) => Comparer.Equals(this, obj);

    /// <summary>
    /// Returns a value indicating whether the collection is equal to another <see cref="HashSet{T}"/>.
    /// </summary>
    /// <param name="other"/>
    /// <returns/>
    // [RecordImp!]: This needs to be public, non-virtual to meet the `record` spec.
    public bool Equals(HashSet<T> other) => Comparer.Equals(this, other);

    /// <summary>
    /// Returns a value indicating whether the collection is equal to another <see cref="RecordSet{T}"/>.
    /// </summary>
    /// <param name="other"/>
    /// <returns/>
    // [RecordImp!]: This needs to be public, non-virtual to meet the `record` spec.
    public virtual bool Equals(RecordSet<T>? other) => Comparer.Equals(this, other);

    /// <summary>
    /// Appends the specified <paramref name="builder"/> with value information for the collection.
    /// </summary>
    /// <param name="builder"></param>
    // [RecordImp!]: This needs to be protected, virtual to meet the `record` spec.
    protected virtual bool PrintMembers(StringBuilder builder)
    {
        RuntimeHelpers.EnsureSufficientExecutionStack();
        builder.Append($"Count = {Count}");

        return true;
    }

    /// <summary>
    /// Returns a value indicating whether two <see cref="RecordSet{T}"/> represent the same collection of records.
    /// </summary>
    // [RecordImp!]: This operator is required to meet the `record` spec.
    public static bool operator ==(RecordSet<T> left, RecordSet<T> right) =>
        left.Equals(right);

    /// <summary>
    /// Returns a value indicating whether two <see cref="RecordSet{T}"/> represent a different collection of records.
    /// </summary>
    // [RecordImp!]: This operator is required to meet the `record` spec.
    public static bool operator !=(RecordSet<T> left, RecordSet<T> right) =>
        !left.Equals(right);

    #endregion

    #region ICollection

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

    #endregion

    #region IEqualityComparer

    /// <summary>
    /// Determines whether the specified objects are equal.
    /// </summary>
    public bool Equals(RecordSet<T>? x, RecordSet<T>? y) =>
        Comparer.Equals(x, y);

    [DebuggerHidden]
    bool IEqualityComparer.Equals(object? x, object? y) =>
        x is RecordSet<T> set && Comparer.Equals(set, y);

    /// <summary>
    /// Returns a hash code for the specified object.
    /// </summary>
    public int GetHashCode(RecordSet<T> x) =>
        Comparer.GetHashCode(x);

    [DebuggerHidden]
    int IEqualityComparer.GetHashCode(object? obj) =>
        obj is RecordSet<T> set ? Comparer.GetHashCode(set) : 0;

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
    //int IComparable.CompareTo(object obj) => obj is RecordSet<T> set ? CompareTo(set) : -1;

    //public int CompareTo(RecordSet<T> other) =>

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
    bool IEquatable<IReadOnlyRecordCollection<T>>.Equals(IReadOnlyRecordCollection<T>? other) =>
        Comparer.Equals(this, other);

    [DebuggerHidden]
    bool IEqualityComparer<IReadOnlyRecordCollection<T>>.Equals(IReadOnlyRecordCollection<T>? x, IReadOnlyRecordCollection<T>? y) =>
        Comparer.Equals(x, y);

    [DebuggerHidden]
    int IEqualityComparer<IRecordCollection<T>>.GetHashCode(IRecordCollection<T> obj) =>
        Comparer.GetHashCode(obj);

    [DebuggerHidden]
    bool IEquatable<IRecordCollection<T>>.Equals(IRecordCollection<T>? other) =>
        Comparer.Equals(this, other);

    [DebuggerHidden]
    bool IEqualityComparer<IRecordCollection<T>>.Equals(IRecordCollection<T>? x, IRecordCollection<T>? y) =>
        Comparer.Equals(x, y);

    [DebuggerHidden]
    int IEqualityComparer<IReadOnlyRecordCollection<T>>.GetHashCode(IReadOnlyRecordCollection<T> obj) =>
        Comparer.GetHashCode(obj);

    #endregion
}
