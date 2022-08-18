namespace System.Collections
{
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
        public static int GetHashCode(IEnumerable? collection)
        {
            // use hash of collection type. Type.GetHashCode is consistent per type
            int startingHash = collection?.GetType().GetHashCode() ?? default;
            int hash = GetHashCode(collection, startingHash);

            return hash;
        }

        /// <summary>
        /// Gets the hashcode of the specified <paramref name="collection"/> elements.
        /// This method can be expensive based on the size of the collection.
        /// </summary>
        /// <param name="collection">The collection whose elements should be hashed.</param>
        /// <param name="startingHash">The starting base hash to calculate the hash against.</param>
        /// <returns>The hash of the collection elements.</returns>
        public static int GetHashCode(IEnumerable? collection, int startingHash)
        {
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
                        hash += (list[i]?.GetHashCode() ?? default) ^ i;
                    }
                }
                else if (collection is IDictionary dictionary)
                {
                    // hash key & value
                    foreach (DictionaryEntry entry in dictionary)
                    {
                        int entryHash = ((entry.Key?.GetHashCode() ?? default) + 1) * ((entry.Value?.GetHashCode() ?? default) + 1);

                        hash += entryHash;
                    }
                }
                else
                {
                    // order is not important
                    foreach (object item in collection)
                    {
                        hash += (item?.GetHashCode() ?? default) ^ 3;
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
        public static bool Equals(ICollection? x, object? y) => y is ICollection collection && Equals(x, x?.Count, collection, collection?.Count);

        /// <summary>
        /// Indicates whether a collection is equal to another object of the same type.
        /// </summary>
        /// <param name="x">The first collection to compare.</param>
        /// <param name="y">The second collection to compare.</param>
        public static bool Equals<T>(Generic.ISet<T>? x, object? y) => y is Generic.ISet<T> collection && Equals(x, x?.Count, collection, collection?.Count);

        /// <summary>
        /// Indicates whether a collection is equal to another object of the same type.
        /// </summary>
        /// <param name="x">The first collection to compare.</param>
        /// <param name="y">The second collection to compare.</param>
        public static bool Equals(ICollection? x, ICollection? y) => Equals(x, x?.Count, y, y?.Count);

        /// <summary>
        /// Indicates whether a collection is equal to another object of the same type.
        /// </summary>
        /// <param name="x">The first collection to compare.</param>
        /// <param name="y">The second collection to compare.</param>
        public static bool Equals<T>(Generic.ISet<T>? x, Generic.ISet<T>? y) => Equals(x, x?.Count, y, y?.Count);

        static bool Equals(IEnumerable? x, int? xCount, IEnumerable? y, int? yCount)
        {
            bool areEqual = xCount == yCount && GetHashCode(x) == GetHashCode(y);

            return areEqual;
        }

        #endregion
    }
}
