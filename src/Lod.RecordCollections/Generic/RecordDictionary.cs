using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Collections.Generic;

/// <summary>
/// Represents a strongly typed dictionary of objects that can be accessed by index.
/// Provides methods to search, sort, and manipulate dictionarys.
/// Record dictionaries support value based comparison of dictionary data.
/// </summary>
/// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
/// <remarks>Uses an underlying collection of <see cref="Dictionary{TKey, TValue}"/>.</remarks>
public class RecordDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IRecordCollection<KeyValuePair<TKey, TValue>>
    , IEnumerable, IEnumerable<KeyValuePair<TKey, TValue>>
    , ICollection, ICollection<KeyValuePair<TKey, TValue>>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    , IEquatable<RecordDictionary<TKey, TValue>>, IEqualityComparer, IEqualityComparer<RecordDictionary<TKey, TValue>>
    //, IComparable, IComparable<RecordDictionary<TKey, TValue>>
    , IStructuralEquatable, IStructuralComparable
    where TValue : IEquatable<TValue>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RecordDictionary{TKey, TValue}"/> class that is empty and has the default initial capacity.
    /// </summary>
    public RecordDictionary() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordDictionary{TKey, TValue}"/> class that uses the specified underlying dictionary.
    /// </summary>
    /// <param name="dictionary">An existing <see cref="RecordDictionary{TKey, TValue}"/> to use as the underlying collection.</param>
    public RecordDictionary(Dictionary<TKey, TValue> dictionary) : base(dictionary) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordDictionary{TKey, TValue}"/> class that
    /// contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new dictionary.</param>
    public RecordDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : base(collection?.ToDictionary(kv => kv.Key, kv => kv.Value)!) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordDictionary{TKey, TValue}"/> class that is empty and has the specified initial capacity.
    /// </summary>
    /// <param name="capacity">The number of elements that the new dictionary can initially store.</param>
    public RecordDictionary(int capacity) : base(new Dictionary<TKey, TValue>(capacity)) { }

    #region Record Specification

    /// <summary>
    /// Gets the record equality contract for this collection.
    /// </summary>
    // [RecordImp!]: This needs to be protected, virtual, returning it's own type to meet the `record` spec.
    protected virtual Type EqualityContract => typeof(RecordDictionary<TKey, TValue>);

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordDictionary{TKey, TValue}"/> class that uses records from an existing collection.
    /// </summary>
    /// <param name="original">An existing <see cref="RecordDictionary{TKey, TValue}"/> to clone into the new record.</param>
    // [RecordImp!]: This needs to be protected, non-null with no null checks to meet the `record` spec.
    protected RecordDictionary(RecordDictionary<TKey, TValue> original)
        : base(original.Select(o => new KeyValuePair<TKey, TValue>(o.Key, RecordCloner.TryClone(o.Value)!)).ToDictionary(kv => kv.Key, kv => kv.Value))
    { }

    /// <inheritdoc/>
    // [RecordImp!]: This needs to be overriden to meet the `record` spec.
    public override int GetHashCode() => RecordCollectionComparer.GetHashCode(this);

    /// <inheritdoc/>
    // [RecordImp!]: This needs to be overriden to meet the `record` spec.
    public override bool Equals(object obj) => RecordCollectionComparer.Equals(this, obj);

    /// <summary>
    /// Returns a value indicating whether the collection is equal to another <see cref="Dictionary{TKey, TValue}"/>.
    /// </summary>
    /// <param name="other"/>
    /// <returns/>
    // [RecordImp!]: This needs to be public, non-virtual to meet the `record` spec.
    public bool Equals(Dictionary<TKey, TValue> other) => RecordCollectionComparer.Equals(this, other);

    /// <summary>
    /// Returns a value indicating whether the collection is equal to another <see cref="RecordDictionary{TKey, TValue}"/>.
    /// </summary>
    /// <param name="other"/>
    /// <returns/>
    // [RecordImp!]: This needs to be public, non-virtual to meet the `record` spec.
    public virtual bool Equals(RecordDictionary<TKey, TValue> other) => RecordCollectionComparer.Equals(this, other);

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
    /// Returns a value indicating whether two <see cref="RecordDictionary{TKey, TValue}"/> represent the same collection of records.
    /// </summary>
    // [RecordImp!]: This operator is required to meet the `record` spec.
    public static bool operator ==(RecordDictionary<TKey, TValue> left, RecordDictionary<TKey, TValue> right) => RecordCollectionComparer.Equals(left, right);

    /// <summary>
    /// Returns a value indicating whether two <see cref="RecordDictionary{TKey, TValue}"/> represent a different collection of records.
    /// </summary>
    // [RecordImp!]: This operator is required to meet the `record` spec.
    public static bool operator !=(RecordDictionary<TKey, TValue> left, RecordDictionary<TKey, TValue> right) => !RecordCollectionComparer.Equals(left, right);

    #endregion

    #region IEqualityComparer

    /// <summary>
    /// Determines whether the specified objects are equal.
    /// </summary>
    /// <param name="x"/>
    /// <param name="y"/>
    /// <returns/>
    public bool Equals(RecordDictionary<TKey, TValue> x, RecordDictionary<TKey, TValue> y) =>
        RecordCollectionComparer.Equals(x, y);

    [DebuggerHidden]
    bool IEqualityComparer.Equals(object x, object y) =>
        x is RecordDictionary<TKey, TValue> set && RecordCollectionComparer.Equals(set, y);

    /// <summary>
    /// Returns a hash code for the specified object.
    /// </summary>
    /// <param name="x"/>
    /// <returns/>
    public int GetHashCode(RecordDictionary<TKey, TValue> x) =>
        RecordCollectionComparer.GetHashCode(x);

    [DebuggerHidden]
    int IEqualityComparer.GetHashCode(object obj) =>
        obj is RecordDictionary<TKey, TValue> set ? RecordCollectionComparer.GetHashCode(set) : 0;

    #endregion

    #region IStructuralEquatable

    [DebuggerHidden]
    bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer) =>
        comparer.Equals(this, other);

    [DebuggerHidden]
    int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) =>
        comparer.GetHashCode(this);

    #endregion

    #region IComparable

    //[DebuggerHidden]
    //int IComparable.CompareTo(object obj) => obj is RecordDictionary<TKey, TValue> set ? CompareTo(set) : -1;

    //public int CompareTo(RecordDictionary<TKey, TValue> other) =>

    #endregion

    #region IStructuralComparable

    [DebuggerHidden]
    int IStructuralComparable.CompareTo(object other, IComparer comparer) =>
        comparer.Compare(this, other);

    #endregion

    #region IRecordCollection

    /// <summary>
    /// Returns a value indicating whether an <paramref name="other"/> collection is equal to the current instance.
    /// </summary>
    /// <param name="other">The collection to compare the current collection to.</param>
    /// <return>True if the underlying collection's elements are equivalent to the current collection.</return>
    public bool Equals(IReadOnlyRecordCollection? other) =>
        RecordCollectionComparer.Equals(this, other);

    /// <summary>
    /// Returns a value indicating whether the <paramref name="left"/> collection is equal to the <paramref name="right"/> collection.
    /// </summary>
    /// <param name="left">The original collection to compare the other collection to.</param>
    /// <param name="right">The collection to compare the current collection to.</param>
    /// <return>True if the underlying collection's elements are equivalent to the current collection.</return>
    public bool Equals(IReadOnlyRecordCollection? left, IReadOnlyRecordCollection? right) =>
        RecordCollectionComparer.Equals(left, right);

    [DebuggerHidden]
    bool IEquatable<IReadOnlyRecordCollection<KeyValuePair<TKey, TValue>>>.Equals(IReadOnlyRecordCollection<KeyValuePair<TKey, TValue>> other) =>
        RecordCollectionComparer.Equals(this, other);

    [DebuggerHidden]
    bool IEqualityComparer<IReadOnlyRecordCollection<KeyValuePair<TKey, TValue>>>.Equals(IReadOnlyRecordCollection<KeyValuePair<TKey, TValue>> x, IReadOnlyRecordCollection<KeyValuePair<TKey, TValue>> y) =>
        RecordCollectionComparer.Equals(x, y);

    [DebuggerHidden]
    int IEqualityComparer<IRecordCollection<KeyValuePair<TKey, TValue>>>.GetHashCode(IRecordCollection<KeyValuePair<TKey, TValue>> obj) =>
        RecordCollectionComparer.GetHashCode(obj);

    [DebuggerHidden]
    bool IEquatable<IRecordCollection<KeyValuePair<TKey, TValue>>>.Equals(IRecordCollection<KeyValuePair<TKey, TValue>> other) =>
        RecordCollectionComparer.Equals(this, other);

    [DebuggerHidden]
    bool IEqualityComparer<IRecordCollection<KeyValuePair<TKey, TValue>>>.Equals(IRecordCollection<KeyValuePair<TKey, TValue>> x, IRecordCollection<KeyValuePair<TKey, TValue>> y) =>
        RecordCollectionComparer.Equals(x, y);

    [DebuggerHidden]
    int IEqualityComparer<IReadOnlyRecordCollection<KeyValuePair<TKey, TValue>>>.GetHashCode(IReadOnlyRecordCollection<KeyValuePair<TKey, TValue>> obj) =>
        RecordCollectionComparer.GetHashCode(obj);

    #endregion
}
