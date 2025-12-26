using System.Collections.Concurrent;

namespace System.Collections;

/// <summary>
/// Provides a base class for implementations of <see cref="IRecordCollectionComparer"/> which exposes methods to support the comparison of record collections.
/// </summary>
public partial class RecordCollectionComparer
    : IRecordCollectionComparer
{
    private static ConcurrentDictionary<Type, IComparisonStrategy> StrategyCache { get; } = [];

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
        if (collection is null) return default; // EqualityComparer<object>.Default.GetHashCode(null) returns 0

        // use hash of collection type. Type.GetHashCode is consistent per type
        int startingHash = collection.GetType().GetHashCode();
        return GetHashCode(collection, startingHash, out _);
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

        if (collection is null) return default;

        IComparisonStrategy strategy = GetStrategy(collection.GetType());
        return strategy.GetHashCode(collection, startingHash);
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
        rollovers = 0;
        unchecked
        {
            int hash = startingHash;
            hash = Combine(hash, list.Count);

            // order is important
            for (int i = 0; i < list.Count; i++)
            {
                int itemHash = list[i]?.GetHashCode() ?? default;
                hash = Combine(hash, Mix(itemHash ^ i));
            }

            return hash;
        }
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
        rollovers = 0;
        unchecked
        {
            int sum = 0;
            int xor = 0;

            foreach (DictionaryEntry entry in dictionary)
            {
                int keyHash = entry.Key?.GetHashCode() ?? default;
                int valueHash = entry.Value?.GetHashCode() ?? default;
                int entryHash = Combine(Mix(keyHash), Mix(valueHash));

                sum += entryHash;
                xor ^= entryHash;
            }

            int hash = startingHash;
            hash = Combine(hash, dictionary.Count);
            hash = Combine(hash, Mix(sum));
            hash = Combine(hash, Mix(xor));
            return hash;
        }
    }

    /// <summary>
    /// Returns the hash of an <see cref="IEnumerable"/>, ignoring order.
    /// </summary>
    /// <param name="startingHash">The starting hash to calculate against.</param>
    /// <param name="collection">The list of elements whose hash will be calculated.</param>
    /// <param name="ignoreOrder">Indicates whether element sequence order is important within the <paramref name="collection"/>.</param>
    /// <param name="rollovers">The number of times the hash rolles over past <see cref="int.MaxValue"/>.</param>
    /// <returns>The hash of the collections elements.</returns>
    protected virtual int GetHashCode(int startingHash, IEnumerable collection, bool ignoreOrder, out int rollovers)
    {
        rollovers = 0;

        unchecked
        {
            int hash = startingHash;
            hash = Combine(hash, ignoreOrder ? 1 : 0);

            if (ignoreOrder)
            {
                int sum = 0;
                int xor = 0;
                int count = 0;
                foreach (object? item in collection)
                {
                    int itemHash = item?.GetHashCode() ?? default;
                    int mixed = Mix(itemHash);
                    sum += mixed;
                    xor ^= mixed;
                    count++;
                }

                hash = Combine(hash, count);
                hash = Combine(hash, Mix(sum));
                hash = Combine(hash, Mix(xor));
                return hash;
            }

            int i = 0;
            foreach (object? item in collection)
            {
                int itemHash = item?.GetHashCode() ?? default;
                hash = Combine(hash, Mix(itemHash ^ i));
                i++;
            }

            hash = Combine(hash, i);
            return hash;
        }
    }

    #endregion

    #region Equals

    /// <summary>
    /// Indicates whether a collection is equal to another object of the same type.
    /// </summary>
    /// <param name="x">The first collection to compare.</param>
    /// <param name="y">The second collection to compare.</param>
    public virtual new bool Equals(object? x, object? y)
    {
        if (ReferenceEquals(x, y)) return true;

        // Only record collections participate in this comparer.
        if (x is IReadOnlyRecordCollection rcx) return Equals(rcx, y);
        if (y is IReadOnlyRecordCollection rcy) return Equals(rcy, x);

        return false;
    }

    /// <summary>
    /// Indicates whether a collection is equal to another object of the same type.
    /// </summary>
    /// <param name="x">The first collection to compare.</param>
    /// <param name="y">The second collection to compare.</param>
    public virtual bool Equals(IReadOnlyRecordCollection? x, object? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return y is null;
        if (y is null) return false;

        // If comparing against another record collection, use the strongly typed path.
        if (y is IReadOnlyRecordCollection rcy) return Equals(x, rcy);

        IComparisonStrategy strategy = GetStrategy(x.GetType());
        return strategy.Equals(x, y);
    }

    /// <summary>
    /// Indicates whether a collection is equal to another object of the same type.
    /// </summary>
    /// <param name="x">The first collection to compare.</param>
    /// <param name="y">The second collection to compare.</param>
    public virtual bool Equals(IReadOnlyRecordCollection? x, IReadOnlyRecordCollection? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        if (x.Count != y.Count) return false;

        // Record equality contract: different collection types are not considered equal.
        if (x.GetType() != y.GetType()) return false;

        IComparisonStrategy strategy = GetStrategy(x.GetType());
        return strategy.Equals(x, y);
    }

    #endregion

    private static IComparisonStrategy GetStrategy(Type recordCollectionType) =>
        StrategyCache.GetOrAdd(recordCollectionType, CreateStrategy);

    private static IComparisonStrategy CreateStrategy(Type recordCollectionType)
    {
        IComparisonStrategy strategy;

        if (TryGetGenericBase(recordCollectionType, typeof(RecordList<>), out Type[]? listArgs) && listArgs?.Length == 1)
        {
            strategy = (IComparisonStrategy)Activator.CreateInstance(typeof(ListStrategy<>).MakeGenericType(listArgs))!;
        }
        else if (TryGetGenericBase(recordCollectionType, typeof(RecordDictionary<,>), out Type[]? dictArgs) && dictArgs?.Length == 2)
        {
            strategy = (IComparisonStrategy)Activator.CreateInstance(typeof(DictionaryStrategy<,>).MakeGenericType(dictArgs))!;
        }
        else if (TryGetGenericBase(recordCollectionType, typeof(RecordSet<>), out Type[]? setArgs) && setArgs?.Length == 1)
        {
            strategy = (IComparisonStrategy)Activator.CreateInstance(typeof(SetStrategy<>).MakeGenericType(setArgs))!;
        }
        else if (TryGetGenericBase(recordCollectionType, typeof(RecordQueue<>), out Type[]? queueArgs) && queueArgs?.Length == 1)
        {
            strategy = (IComparisonStrategy)Activator.CreateInstance(typeof(OrderedEnumerableStrategy<>).MakeGenericType(queueArgs))!;
        }
        else if (TryGetGenericBase(recordCollectionType, typeof(RecordStack<>), out Type[]? stackArgs) && stackArgs?.Length == 1)
        {
            strategy = (IComparisonStrategy)Activator.CreateInstance(typeof(OrderedEnumerableStrategy<>).MakeGenericType(stackArgs))!;
        }
        else
        {
            strategy = new DefaultStrategy();
        }

        return strategy;
    }

    private static bool TryGetGenericBase(Type candidate, Type openGenericBase, out Type[]? genericArguments)
    {
        for (Type? t = candidate; t != null; t = t.BaseType)
        {
            if (!t.IsGenericType) continue;

            Type def = t.GetGenericTypeDefinition();
            if (def != openGenericBase) continue;

            genericArguments = t.GetGenericArguments();
            return true;
        }

        genericArguments = null;
        return false;
    }

    private static int Combine(int hash, int value) =>
        unchecked((hash * 16777619) ^ value);

    // A small, fast integer mixing function (xorshift-based) to reduce clustering in commutative hashes.
    private static int Mix(int value)
    {
        unchecked
        {
            uint x = (uint)value;
            x ^= x >> 16;
            x *= 0x7feb352d;
            x ^= x >> 15;
            x *= 0x846ca68b;
            x ^= x >> 16;
            return (int)x;
        }
    }

    private interface IComparisonStrategy
    {
        public bool Equals(IReadOnlyRecordCollection x, object y);

        public int GetHashCode(IReadOnlyRecordCollection x, int startingHash);
    }
}
