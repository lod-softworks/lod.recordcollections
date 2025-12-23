namespace System.Collections;

/// <summary>
/// Provides a base class for implementations of <see cref="IRecordCollectionComparer"/> which exposes methods to support the comparison of record collections.
/// </summary>
public class RecordCollectionComparer
    : IRecordCollectionComparer
{
    /// <summary>
    /// Gets the default comparer for record collections.
    /// </summary>
    /// <remarks>
    /// This comparer is used for record collections initialized without specifying a <see cref="IRecordCollectionComparer"/> in their constructor.
    /// In .NET 6.0 or greater, this property proxies to the default comparer defined on <see cref="IReadOnlyRecordCollection"/>.
    /// </remarks>
    public static IRecordCollectionComparer Default
#if NET6_0_OR_GREATER
    {
        get => IReadOnlyRecordCollection.DefaultComparer;
        [Obsolete("Use IReadOnlyRecordCollection.DefaultComparer instead.")]
        set => IReadOnlyRecordCollection.DefaultComparer = value;
    }
#else
    { get; set; } = new RecordCollectionComparer();
#endif

    #region GetHashCode

    /// <summary>
    /// Gets the hashcode of the specified <paramref name="obj"/> elements if it's a record collection.
    /// This method can be expensive based on the size of the collection.
    /// </summary>
    /// <param name="obj">The collection whose elements should be hashed.</param>
    /// <returns>The hash of the collection elements.</returns>
    public int GetHashCode(object? obj) =>
        obj is IReadOnlyRecordCollection collection ? GetHashCode(collection) : default;

    /// <summary>
    /// Gets the hashcode of the specified <paramref name="collection"/> elements.
    /// This method can be expensive based on the size of the collection.
    /// </summary>
    /// <param name="collection">The collection whose elements should be hashed.</param>
    /// <returns>The hash of the collection elements.</returns>
    public int GetHashCode(IReadOnlyRecordCollection? collection)
    {
        // use hash of collection type. Type.GetHashCode is consistent per type
        int startingHash = collection?.GetType().GetHashCode() ?? default;
        int hash = GetHashCode(collection, startingHash, out _);

        return hash;
    }

    /// <summary>
    /// Gets the hashcode of the specified <paramref name="collection"/> elements.
    /// This method can be expensive based on the size of the collection.
    /// </summary>
    /// <param name="collection">The collection whose elements should be hashed.</param>
    /// <param name="startingHash">The starting base hash to calculate the hash against.</param>
    /// <param name="rollovers">The number of times the hash has exceeded <see cref="int.MaxValue"/>.</param>
    /// <returns>The hash of the collection elements.</returns>
    protected virtual int GetHashCode(IReadOnlyRecordCollection? collection, int startingHash, out int rollovers)
    {
        rollovers = 0;

        if (collection == null) return default; // EqualityComparer<object>.Default.GetHashCode(null) returns 0

        int hash;
        unchecked
        {
            if (collection is IList list)
            {
                hash = GetHashCode(startingHash, list, out rollovers);
            }
            else if (collection is IDictionary dictionary)
            {
                hash = GetHashCode(startingHash, dictionary, out rollovers);
            }
            else
            {
                hash = GetHashCode(startingHash, collection, out rollovers);
            }
        }

        return hash;
    }

    /// <summary>
    /// Returns the hash of an <see cref="IList"/>, taking ordering into consideration.
    /// </summary>
    /// <param name="startingHash">The starting hash to calculate against.</param>
    /// <param name="list">The list of elements whose hash will be calculated.</param>
    /// <param name="rollovers">The number of times the hash rolles over past <see cref="int.MaxValue"/>.</param>
    /// <returns>The hash of the collections elements.</returns>
    protected virtual int GetHashCode(int startingHash, IList list, out int rollovers)
    {
        int hash = startingHash;
        rollovers = 0;

        // order is important
        for (int i = list.Count - 1; i >= 0; i--)
        {
            int oldHash = hash;
            hash += (list[i]?.GetHashCode() ?? default) ^ i;

            if (oldHash > hash)
            {
                rollovers += 1;
            }
        }

        return hash;
    }

    /// <summary>
    /// Returns the hash of an <see cref="IDictionary"/>, hashing the keys and values.
    /// </summary>
    /// <param name="startingHash">The starting hash to calculate against.</param>
    /// <param name="dictionary">The list of elements whose hash will be calculated.</param>
    /// <param name="rollovers">The number of times the hash rolles over past <see cref="int.MaxValue"/>.</param>
    /// <returns>The hash of the collections elements.</returns>
    protected virtual int GetHashCode(int startingHash, IDictionary dictionary, out int rollovers)
    {
        int hash = startingHash;
        rollovers = 0;

        // hash key & value
        foreach (DictionaryEntry entry in dictionary)
        {
            int oldHash = hash;
            int entryHash = ((entry.Key?.GetHashCode() ?? default) + 1) * ((entry.Value?.GetHashCode() ?? default) + 1);

            hash += entryHash;

            if (oldHash > hash)
            {
                rollovers += 1;
            }
        }

        return hash;
    }

    /// <summary>
    /// Returns the hash of an <see cref="IEnumerable"/>, ignoring order.
    /// </summary>
    /// <param name="startingHash">The starting hash to calculate against.</param>
    /// <param name="collection">The list of elements whose hash will be calculated.</param>
    /// <param name="rollovers">The number of times the hash rolles over past <see cref="int.MaxValue"/>.</param>
    /// <returns>The hash of the collections elements.</returns>
    protected virtual int GetHashCode(int startingHash, IEnumerable collection, out int rollovers)
    {
        int hash = startingHash;
        rollovers = 0;

        // order is not important
        foreach (object item in collection)
        {
            int oldHash = hash;
            hash += (item?.GetHashCode() ?? default) ^ 3;

            if (oldHash > hash)
            {
                rollovers += 1;
            }
        }

        return hash;
    }

    #endregion

    #region Equals

    /// <summary>
    /// Indicates whether a collection is equal to another object of the same type.
    /// </summary>
    /// <param name="x">The first collection to compare.</param>
    /// <param name="y">The second collection to compare.</param>
    public virtual new bool Equals(object? x, object? y) =>
        x is IReadOnlyRecordCollection xCollection && y is IReadOnlyRecordCollection yCollection && Equals(xCollection, yCollection);

    /// <summary>
    /// Indicates whether a collection is equal to another object of the same type.
    /// </summary>
    /// <param name="x">The first collection to compare.</param>
    /// <param name="y">The second collection to compare.</param>
    public virtual bool Equals(IReadOnlyRecordCollection? x, object? y) =>
        y is IReadOnlyRecordCollection collection && Equals(x, collection);

    /// <summary>
    /// Indicates whether a collection is equal to another object of the same type.
    /// </summary>
    /// <param name="x">The first collection to compare.</param>
    /// <param name="y">The second collection to compare.</param>
    public virtual bool Equals(IReadOnlyRecordCollection? x, IReadOnlyRecordCollection? y) =>
        x?.Count == y?.Count && GetHashCode(x) == GetHashCode(y);

    #endregion
}
