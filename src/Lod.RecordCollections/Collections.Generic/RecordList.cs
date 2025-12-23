using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Collections.Generic;

/// <summary>
/// Represents a strongly typed list of objects that can be accessed by index.
/// Provides methods to search, sort, and manipulate lists.
/// Record lists support value based comparison.
/// </summary>
/// <typeparam name="T">The type of elements in the list.</typeparam>
public class RecordList<T> : List<T>, IRecordCollection<T>
    , IEnumerable, IEnumerable<T>
    , ICollection, ICollection<T>, IReadOnlyCollection<T>
    , IEquatable<RecordList<T>>, IEqualityComparer, IEqualityComparer<RecordList<T>>
    //, IComparable, IComparable<RecordList<T>>
    , IStructuralEquatable, IStructuralComparable
    where T : IEquatable<T>
{
    /// <summary>
    /// Gets the comparer used to compare elements and collections.
    /// </summary>
    protected virtual IRecordCollectionComparer Comparer { get; } = new RecordCollectionComparer();

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordList{T}"/> class that is empty and has the default initial capacity.
    /// </summary>
    public RecordList() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordList{T}"/> class that uses the specified underlying list.
    /// </summary>
    /// <param name="list">An existing <see cref="List{T}"/> to use as the underlying collection.</param>
    public RecordList(List<T> list) : base(list) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordList{T}"/> class that
    /// contains elements copied from the specified collection and has sufficient capacity
    /// to accommodate the number of elements copied.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new list.</param>
    public RecordList(IEnumerable<T> collection) : base(collection) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordList{T}"/> class that is empty and has the specified initial capacity.
    /// </summary>
    /// <param name="capacity">The number of elements that the new list can initially store.</param>
    public RecordList(int capacity) : base(capacity) { }

    #region Record Specification

    /// <summary>
    /// Gets the record equality contract for this collection.
    /// </summary>
    // [RecordImp!]: This needs to be protected, virtual, returning it's own type to meet the `record` spec.
    protected virtual Type EqualityContract => typeof(RecordList<T>);

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordList{T}"/> class that uses records from an existing collection.
    /// </summary>
    /// <param name="original">An existing <see cref="RecordList{T}"/> to clone into the new record.</param>
    // [RecordImp!]: This needs to be protected, non-null with no null checks to meet the `record` spec.
    protected RecordList(RecordList<T> original) : base(original.Select(o => RecordCloner.TryClone(o)!)) { }

    /// <inheritdoc/>
    // [RecordImp!]: This needs to be overriden to meet the `record` spec.
    public override int GetHashCode() => Comparer.GetHashCode(this);

    /// <inheritdoc/>
    // [RecordImp!]: This needs to be overriden to meet the `record` spec.
    public override bool Equals(object? obj) => Comparer.Equals(this, obj);

    /// <summary>
    /// Returns a value indicating whether the collection is equal to another <see cref="List{T}"/>.
    /// </summary>
    /// <param name="other"/>
    /// <returns/>
    // [RecordImp!]: This needs to be public, non-virtual to meet the `record` spec.
    public bool Equals(List<T> other) => Comparer.Equals(this, other);

    /// <summary>
    /// Returns a value indicating whether the collection is equal to another <see cref="RecordList{T}"/>.
    /// </summary>
    /// <param name="other"/>
    /// <returns/>
    // [RecordImp!]: This needs to be public, non-virtual to meet the `record` spec.
    public virtual bool Equals(RecordList<T>? other) => Comparer.Equals(this, other);

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
    /// Returns a value indicating whether two <see cref="RecordList{T}"/> represent the same collection of records.
    /// </summary>
    // [RecordImp!]: This operator is required to meet the `record` spec.
    public static bool operator ==(RecordList<T> left, RecordList<T> right) => RecordCollectionComparer.Default.Equals(left, right);

    /// <summary>
    /// Returns a value indicating whether two <see cref="RecordList{T}"/> represent a different collection of records.
    /// </summary>
    // [RecordImp!]: This operator is required to meet the `record` spec.
    public static bool operator !=(RecordList<T> left, RecordList<T> right) => !RecordCollectionComparer.Default.Equals(left, right);

    #endregion

    #region IEqualityComparer

    /// <summary>
    /// Determines whether the specified objects are equal.
    /// </summary>
    /// <param name="x"/>
    /// <param name="y"/>
    /// <returns/>
    public bool Equals(RecordList<T>? x, RecordList<T>? y) =>
        Comparer.Equals(x, y);

    [DebuggerHidden]
    bool IEqualityComparer.Equals(object? x, object? y) =>
        x is RecordList<T> set && Comparer.Equals(set, y);

    /// <summary>
    /// Returns a hash code for the specified object.
    /// </summary>
    /// <param name="x"/>
    /// <returns/>
    public int GetHashCode(RecordList<T> x) =>
        Comparer.GetHashCode(x);

    [DebuggerHidden]
    int IEqualityComparer.GetHashCode(object obj) =>
        obj is RecordList<T> set ? Comparer.GetHashCode(set) : 0;

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
    //int IComparable.CompareTo(object obj) => obj is RecordList<T> set ? CompareTo(set) : -1;

    //public int CompareTo(RecordList<T> other) =>

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
