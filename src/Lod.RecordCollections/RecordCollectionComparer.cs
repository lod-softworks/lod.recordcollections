namespace System.Collections;

/// <summary>
/// A static utility class containing methods for comparing value collections.
/// </summary>
public static class RecordCollectionComparer
{
    #region GetHashCode

    /// <summary>
    /// Gets the hashcode of the specified <paramref name="collection"/> elements.
    /// This method can be expensive based on the size of the collection.
    /// </summary>
    /// <param name="collection">The collection whose elements should be hashed.</param>
    /// <returns>The hash of the collection elements.</returns>
    public static int GetHashCode(ICollection? collection)
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
    public static int GetHashCode(ICollection? collection, int startingHash, out int rollovers)
    {
        rollovers = 0;

        if (collection == null) return default; // EqualityComparer<object>.Default.GetHashCode(null) returns 0

        // use hash of collection type. Type.GetHashCode is consistent per type
        int hash = startingHash;

        unchecked
        {
            if (collection is IList list)
            {
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
            }
            else if (collection is IDictionary dictionary)
            {
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
            }
            else
            {
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
    public static bool Equals(ICollection? x, object? y) => y is ICollection collection && Equals(x, collection);

    /// <summary>
    /// Indicates whether a collection is equal to another object of the same type.
    /// </summary>
    /// <param name="x">The first collection to compare.</param>
    /// <param name="y">The second collection to compare.</param>
    public static bool Equals(ICollection? x, ICollection? y)
    {
        bool areEqual = x?.Count == y?.Count && GetHashCode(x) == GetHashCode(y);

        return areEqual;
    }

    #endregion
}
