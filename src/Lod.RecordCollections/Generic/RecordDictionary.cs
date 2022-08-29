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
public class RecordDictionary<TKey, TValue> : Dictionary<TKey, TValue>
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
}
