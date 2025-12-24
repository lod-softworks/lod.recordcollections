using System.Collections.Concurrent;
using System.Collections.Generic;

namespace System.Collections;

/// <summary>
/// Provides a base class for implementations of <see cref="IRecordCollectionComparer"/> which exposes methods to support the comparison of record collections.
/// </summary>
public class RecordCollectionComparer
    : IRecordCollectionComparer
{
    private static readonly ConcurrentDictionary<Type, IComparisonStrategy> StrategyCache = new();

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
        if (TryGetGenericBase(recordCollectionType, typeof(global::System.Collections.Generic.RecordList<>), out Type[]? listArgs))
        {
            return (IComparisonStrategy)Activator.CreateInstance(typeof(ListStrategy<>).MakeGenericType(listArgs))!;
        }

        if (TryGetGenericBase(recordCollectionType, typeof(global::System.Collections.Generic.RecordQueue<>), out Type[]? queueArgs))
        {
            return (IComparisonStrategy)Activator.CreateInstance(typeof(QueueStrategy<>).MakeGenericType(queueArgs))!;
        }

        if (TryGetGenericBase(recordCollectionType, typeof(global::System.Collections.Generic.RecordStack<>), out Type[]? stackArgs))
        {
            return (IComparisonStrategy)Activator.CreateInstance(typeof(StackStrategy<>).MakeGenericType(stackArgs))!;
        }

        if (TryGetGenericBase(recordCollectionType, typeof(global::System.Collections.Generic.RecordSet<>), out Type[]? setArgs))
        {
            return (IComparisonStrategy)Activator.CreateInstance(typeof(SetStrategy<>).MakeGenericType(setArgs))!;
        }

        if (TryGetGenericBase(recordCollectionType, typeof(global::System.Collections.Generic.RecordDictionary<,>), out Type[]? dictArgs))
        {
            return (IComparisonStrategy)Activator.CreateInstance(typeof(DictionaryStrategy<,>).MakeGenericType(dictArgs))!;
        }

        return new FallbackStrategy();
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

    private interface IComparisonStrategy
    {
        bool Equals(IReadOnlyRecordCollection x, object y);
        int GetHashCode(IReadOnlyRecordCollection x, int startingHash);
    }

    private sealed class ListStrategy<T> : IComparisonStrategy
    {
        public bool Equals(IReadOnlyRecordCollection x, object y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (y is null) return false;

            if (x is not IList<T> listX) return false;
            if (y is not IList<T> listY) return false;
            if (listX.Count != listY.Count) return false;

            EqualityComparer<T> eq = EqualityComparer<T>.Default;
            for (int i = 0; i < listX.Count; i++)
            {
                if (!eq.Equals(listX[i], listY[i])) return false;
            }

            return true;
        }

        public int GetHashCode(IReadOnlyRecordCollection x, int startingHash)
        {
            if (x is not IList<T> listX) return startingHash;

            unchecked
            {
                int hash = startingHash;
                hash = Combine(hash, listX.Count);

                EqualityComparer<T> eq = EqualityComparer<T>.Default;
                for (int i = 0; i < listX.Count; i++)
                {
                    int itemHash = eq.GetHashCode(listX[i]!);
                    hash = Combine(hash, Mix(itemHash ^ i));
                }

                return hash;
            }
        }
    }

    private sealed class QueueStrategy<T> : IComparisonStrategy
    {
        public bool Equals(IReadOnlyRecordCollection x, object y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (y is null) return false;

            if (x is not IEnumerable<T> seqX) return false;
            if (y is not IEnumerable<T> seqY) return false;

            EqualityComparer<T> eq = EqualityComparer<T>.Default;

            using (IEnumerator<T> e1 = seqX.GetEnumerator())
            using (IEnumerator<T> e2 = seqY.GetEnumerator())
            {
                while (true)
                {
                    bool m1 = e1.MoveNext();
                    bool m2 = e2.MoveNext();
                    if (m1 != m2) return false;
                    if (!m1) return true;
                    if (!eq.Equals(e1.Current, e2.Current)) return false;
                }
            }
        }

        public int GetHashCode(IReadOnlyRecordCollection x, int startingHash)
        {
            if (x is not IEnumerable<T> seqX) return startingHash;

            unchecked
            {
                int hash = startingHash;
                int i = 0;
                EqualityComparer<T> eq = EqualityComparer<T>.Default;

                foreach (T item in seqX)
                {
                    int itemHash = eq.GetHashCode(item!);
                    hash = Combine(hash, Mix(itemHash ^ i));
                    i++;
                }

                hash = Combine(hash, i);
                return hash;
            }
        }
    }

    private sealed class StackStrategy<T> : QueueStrategy<T>
    {
        // Same semantics as Queue: order is defined by the enumerator, and matters.
    }

    private sealed class SetStrategy<T> : IComparisonStrategy
    {
        public bool Equals(IReadOnlyRecordCollection x, object y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (y is null) return false;

            if (x is not ISet<T> setX) return false;
            if (y is not ISet<T> setY) return false;
            if (setX.Count != setY.Count) return false;

            return setX.SetEquals(setY);
        }

        public int GetHashCode(IReadOnlyRecordCollection x, int startingHash)
        {
            if (x is not IEnumerable<T> seqX) return startingHash;

            unchecked
            {
                int sum = 0;
                int xor = 0;
                int count = 0;
                EqualityComparer<T> eq = EqualityComparer<T>.Default;

                foreach (T item in seqX)
                {
                    int itemHash = eq.GetHashCode(item!);
                    int mixed = Mix(itemHash);
                    sum += mixed;
                    xor ^= mixed;
                    count++;
                }

                int hash = startingHash;
                hash = Combine(hash, count);
                hash = Combine(hash, Mix(sum));
                hash = Combine(hash, Mix(xor));
                return hash;
            }
        }
    }

    private sealed class DictionaryStrategy<TKey, TValue> : IComparisonStrategy
        where TKey : notnull
    {
        public bool Equals(IReadOnlyRecordCollection x, object y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (y is null) return false;

            if (x is not IDictionary<TKey, TValue> dictX) return false;
            if (y is not IDictionary<TKey, TValue> dictY) return false;
            if (dictX.Count != dictY.Count) return false;

            EqualityComparer<TValue> eq = EqualityComparer<TValue>.Default;

            if (dictY is Dictionary<TKey, TValue> concreteY)
            {
                foreach (KeyValuePair<TKey, TValue> kv in dictX)
                {
                    if (!concreteY.TryGetValue(kv.Key, out TValue? otherValue)) return false;
                    if (!eq.Equals(kv.Value, otherValue)) return false;
                }

                return true;
            }

            foreach (KeyValuePair<TKey, TValue> kv in dictX)
            {
                if (!dictY.ContainsKey(kv.Key)) return false;
                if (!eq.Equals(kv.Value, dictY[kv.Key])) return false;
            }

            return true;
        }

        public int GetHashCode(IReadOnlyRecordCollection x, int startingHash)
        {
            if (x is not IEnumerable<KeyValuePair<TKey, TValue>> seqX) return startingHash;

            unchecked
            {
                int sum = 0;
                int xor = 0;
                int count = 0;

                EqualityComparer<TKey> keyEq = EqualityComparer<TKey>.Default;
                EqualityComparer<TValue> valueEq = EqualityComparer<TValue>.Default;

                foreach (KeyValuePair<TKey, TValue> kv in seqX)
                {
                    int keyHash = keyEq.GetHashCode(kv.Key);
                    int valueHash = valueEq.GetHashCode(kv.Value!);
                    int entryHash = Combine(Mix(keyHash), Mix(valueHash));

                    sum += entryHash;
                    xor ^= entryHash;
                    count++;
                }

                int hash = startingHash;
                hash = Combine(hash, count);
                hash = Combine(hash, Mix(sum));
                hash = Combine(hash, Mix(xor));
                return hash;
            }
        }
    }

    private sealed class FallbackStrategy : IComparisonStrategy
    {
        public bool Equals(IReadOnlyRecordCollection x, object y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (y is null) return false;

            if (x is IList listX && y is IList listY)
            {
                if (listX.Count != listY.Count) return false;
                for (int i = 0; i < listX.Count; i++)
                {
                    if (!object.Equals(listX[i], listY[i])) return false;
                }
                return true;
            }

            if (x is IDictionary dictX && y is IDictionary dictY)
            {
                if (dictX.Count != dictY.Count) return false;

                foreach (DictionaryEntry entry in dictX)
                {
                    if (!dictY.Contains(entry.Key)) return false;
                    if (!object.Equals(dictY[entry.Key], entry.Value)) return false;
                }

                return true;
            }

            if (x is IEnumerable seqX && y is IEnumerable seqY)
            {
                IEnumerator e1 = seqX.GetEnumerator();
                IEnumerator e2 = seqY.GetEnumerator();

                while (true)
                {
                    bool m1 = e1.MoveNext();
                    bool m2 = e2.MoveNext();
                    if (m1 != m2) return false;
                    if (!m1) return true;
                    if (!object.Equals(e1.Current, e2.Current)) return false;
                }
            }

            return false;
        }

        public int GetHashCode(IReadOnlyRecordCollection x, int startingHash)
        {
            if (x is IList list)
            {
                unchecked
                {
                    int hash = startingHash;
                    hash = Combine(hash, list.Count);

                    for (int i = 0; i < list.Count; i++)
                    {
                        int itemHash = list[i]?.GetHashCode() ?? default;
                        hash = Combine(hash, Mix(itemHash ^ i));
                    }

                    return hash;
                }
            }

            if (x is IDictionary dictionary)
            {
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

            if (x is IEnumerable enumerable)
            {
                unchecked
                {
                    int hash = startingHash;
                    int i = 0;

                    foreach (object? item in enumerable)
                    {
                        int itemHash = item?.GetHashCode() ?? default;
                        hash = Combine(hash, Mix(itemHash ^ i));
                        i++;
                    }

                    hash = Combine(hash, i);
                    return hash;
                }
            }

            return startingHash;
        }
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
}
